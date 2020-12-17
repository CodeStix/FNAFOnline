using Stx.Net.RoomBased;
using Stx.Serialization;
using System;
using System.Collections;
using System.Linq;

namespace Stx.Net.ServerOnly.RoomBased
{
    public class ServerRoom
    {
        public Server Server { get; private set; }
        /// <summary>
        /// The actual underlaying room.
        /// </summary>
        public Room Underlaying { get; private set; }
        /// <summary>
        /// The hashed password of this room.
        /// </summary>
        public string HashedRoomPassword { get; private set; } = null;
        /// <summary>
        /// Is this room hidden for matchmaking requests?
        /// </summary>
        public bool Hidden { get; set; } = false;
        /// <summary>
        /// Gets/sets the bound application object from this room.
        /// </summary>
        public RoomApplication Application
        {
            get
            {
                return boundApplication;
            }
            set
            {
                boundApplication = value;
                boundApplication.ServerRoom = this;
                boundApplication.OnInitialize();
            }
        }
        /// <summary>
        /// The ID of the underlaying room.
        /// </summary>
        public string ID
        {
            get
            {
                return Underlaying.ID;
            }
        }
        /// <summary>
        /// Should this room get destroyed? 
        /// Criteria for this to return true:
        /// <para>The lifespan of this room is longer than 2 days, or the amount of players is 0 and the total lifespan is longer than 10 seconds.</para>
        /// </summary>
        public bool ShouldBeDisposed
        {
            get
            {
                TimeSpan life = DateTime.UtcNow - Underlaying.CreationTime;
                return (life.TotalHours >= 2.0d) || (Underlaying.PlayerCount <= 0 && life.TotalSeconds > 10.0d);
            }
        }

        public static event Action<ClientData, ServerRoom> OnJoinRoom;
        public static event Action<ClientData, ServerRoom> OnLeaveRoom;

        private RoomApplication boundApplication = RoomApplication.Default;

        public ServerRoom(Server server, RoomTemplate roomTemplate, INetworkedItem owner, string roomCode = null)
        {
            Server = server;

            Underlaying = new Room(roomTemplate, owner, roomCode);
            Hidden = roomTemplate.Hidden;

            if (roomTemplate.Locked)
                HashedRoomPassword = SecurePasswordHasher.Hash(roomTemplate.Password);
        }

        public ServerRoom(Server server, RoomTemplate roomTemplate, NetworkIdentity owner, string roomCode = null)
        {
            Server = server;

            Underlaying = new Room(roomTemplate, owner, roomCode);
            Hidden = roomTemplate.Hidden;

            if (roomTemplate.Locked)
                HashedRoomPassword = SecurePasswordHasher.Hash(roomTemplate.Password);
        }

        public bool HasAccess(string withPassword)
        {
            if (!Underlaying.Locked || string.IsNullOrEmpty(HashedRoomPassword))
                return true;

            if (Underlaying.Locked && string.IsNullOrWhiteSpace(withPassword))
                return false;

            return SecurePasswordHasher.Verify(withPassword, HashedRoomPassword);
        }

        public bool TryBroadcast(string asSenderID, Hashtable data, bool excludeSender = true, params string[] receivers)
        {
            if (!Underlaying.Contains(asSenderID) && asSenderID != Server.NetworkID)
                return false;

            if (receivers == null)
                receivers = new string[0];

            Underlaying.ConnectedClients.ForEach((client) =>
            {
                if (receivers.Length <= 0 || receivers.Contains(client))
                {
                    if (excludeSender && client == asSenderID)
                        return;

                    ClientData d = Server.GetConnectedClient(client);

                    if (d == null)
                        return;

                    Packet p = new Packet(asSenderID);
                    p.Data = new BHashtable(data);
                    Server.Send(d, p.ToBytes());
                }
            });

            return true;
        }

        public void BroadcastAsServer(Hashtable data, params string[] receivers)
        {
            if (receivers == null)
                receivers = new string[0];

            Underlaying.ConnectedClients.ForEach((client) =>
            {
                if (receivers.Length <= 0 || receivers.Contains(client))
                {
                    ClientData d = Server.GetConnectedClient(client);

                    if (d == null)
                        return;

                    Packet p = new Packet(Server.NetworkID);
                    p.Data = new BHashtable(data);
                    Server.Send(d, p.ToBytes());
                }
            });
        }

        public bool TryChangeGameState(GameState newGameState)
        {
            if (Underlaying.State == newGameState)
                return false;

            Underlaying.State = newGameState;

            Underlaying.ConnectedClients.ForEach((client) =>
            {
                var cd = Server.GetConnectedClient(client);

                if (cd != null)
                {
                    if (newGameState == GameState.InGame)
                        cd.Identity.CameInGame();
                    else if (newGameState == GameState.InLobby)
                        cd.Identity.CameBackLobby();

                    cd.Save();
                }

            });

            if (newGameState == GameState.InGame)
                Application.OnStart();
            else if (newGameState == GameState.InLobby)
                Application.OnEnd();

            return TryBroadcast(Server.NetworkID, new Hashtable()
            {
                ["_GameStateChanged"] = newGameState
            });
        }

        public bool TryJoin(ClientData client, string password = null)
        {
            if (Underlaying.IsFull || client.Identity.IsInAnyRoom || !HasAccess(password))
            {
                return false;
            }

            client.Identity.CameInRoom(Underlaying);
            client.Save();

            BroadcastAsServer(new Hashtable()
            {
                ["_SomeoneJoinedRoom"] = client.Identity
            });
            
            Underlaying.ConnectedClients.Add(client.NetworkID);

            Application.OnJoined(client);
            OnJoinRoom?.Invoke(client, this);

            return true;
        }

        public void ForceJoin(ClientData client)
        {
            BroadcastAsServer(new Hashtable()
            {
                ["_SomeoneJoinedRoom"] = client.Identity
            });

            client.Identity.CameInRoom(Underlaying);
            client.Save();

            Underlaying.ConnectedClients.Add(client.NetworkID);

            Application.OnJoined(client);
            OnJoinRoom?.Invoke(client, this);
        }

        public bool TryRemove(ClientData client, bool notifyGotKicked = false)
        {
            client.Identity.CamePreLobby();
            client.Save();

            if (client == null || !Underlaying.Contains(client.NetworkID))
                return false;

            Underlaying.ConnectedClients.Remove(client.NetworkID);

            if (Underlaying.ConnectedClients.Count > 0)
            {
                if (Underlaying.OwnerClientID == client.NetworkID)
                {
                    //Logger.Log("Making a new owner... current: " + room.Underlaying.OwnerClientID, LoggedImportance.Debug);

                    Underlaying.OwnerClientID = Underlaying.ConnectedClients[0];

                    Application.OnPromoted(Underlaying.OwnerClientID);

                    BroadcastAsServer(new Hashtable()
                    {
                        ["_SomeonePromoted"] = Underlaying.OwnerClientID
                    });

                    //Logger.Log("Made a new owner. new: " + room.Underlaying.OwnerClientID, LoggedImportance.Debug);
                }

                Application.OnLeft(client);
                OnLeaveRoom?.Invoke(client, this);

                BroadcastAsServer(new Hashtable()
                {
                    ["_SomeoneLeftRoom"] = client.Identity
                });
            }

            if (notifyGotKicked)
                Server.Send(client, Packet.SingleDataPacket(Server.NetworkID, "_KickedFromRoom", true).ToBytes());

            return true;
        }

        public void KickAll()
            => KickAsServer();

        public void KickAsServer(ClientData clientToKick)
        {
            if (Underlaying.PlayerCount == 0)
                return;

            TryRemove(clientToKick, true);
        }

        public void KickAsServer(params string[] clientIDsToKick)
        {
            if (Underlaying.PlayerCount == 0)
                return;

            if (clientIDsToKick == null || clientIDsToKick.Length <= 0)
            {
                //Add everyone to kick list
                clientIDsToKick = Underlaying.ConnectedClients.ToArray();
            }

            foreach (string ctk in clientIDsToKick)
            {
                ClientData d = Server.GetConnectedClient(ctk);
                TryRemove(d, true);
            }
        }

        public bool TryKick(string senderID, params string[] toKick)
        {
            if ((Underlaying.IsOwner(senderID) || senderID == Server.NetworkID) && Underlaying.PlayerCount > 0)
            {
                KickAsServer(toKick);

                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return Underlaying.ToString();
        }
    }
}
