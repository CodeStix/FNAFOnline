using Stx.Serialization;
using Stx.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stx.Net.RoomBased
{
    public class Room : IByteDefined<Room>
    {
        /// <summary>
        /// The unique ID of this room.
        /// </summary>
        public string ID { get; private set; }
        /// <summary>
        /// The ID of this room's owner.
        /// </summary>
        public string OwnerClientID { get; set; }
        /// <summary>
        /// A list of client that are currently in this room.
        /// </summary>
        public BConcurrentList<string> ConnectedClients { get; set; } = new BConcurrentList<string>();
        /// <summary>
        /// The date and time from when this room got hosted.
        /// </summary>
        public DateTime CreationTime { get; private set; }
        /// <summary>
        /// A list of tags this room consists of, used to find specific uses for this room in matchmaking.
        /// </summary>
        public BList<string> Tags { get; set; }
        /// <summary>
        /// The display name of this room.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The maximum amount of players this room can and should hold.
        /// </summary>
        public int MaxPlayers { get; private set; }
        /// <summary>
        /// Is this room locked with a password?
        /// </summary>
        public bool Locked { get; private set; }
        /// <summary>
        /// The code of this room, used to let others easily join this room.
        /// </summary>
        public string RoomCode { get; set; } = null;
        /// <summary>
        /// The amount of players that is required to start the game.
        /// </summary>
        public int RequiredPlayerCount { get; set; } = 2;
        /// <summary>
        /// A key-value collection of clients and their tags in this room.
        /// </summary>
        public BHashtable TaggedClients { get; set; } = new BHashtable();
        /// <summary>
        /// The current state of the game in this room.
        /// </summary>
        public GameState State { get; set; }
        /// <summary>
        /// Is this room full?
        /// </summary>
        [DoNotSerialize]
        public bool IsFull
        {
            get
            {
                return PlayerCount >= MaxPlayers;
            }
        }
        /// <summary>
        /// How many online players are already in this room?
        /// </summary>
        [DoNotSerialize]
        public int PlayerCount
        {
            get
            {
                return ConnectedClients.Count;
            }
        }
        /// <summary>
        /// Are there enough players to start the game?
        /// </summary>
        [DoNotSerialize]
        public bool EnoughPlayers
        {
            get
            {
                return PlayerCount >= RequiredPlayerCount;
            }
        }
        /// <summary>
        /// The display name of the owner of this room.
        /// </summary>
        public string OwnerDisplayName { get; set; }

        public Room()
        { }

        public Room(RoomTemplate roomTemplate, INetworkedItem owner, string roomCode = null)
        {
            ID = Guid.NewGuid().ToString();
            OwnerClientID = owner.NetworkID;
            OwnerDisplayName = owner.NetworkID;
            CreationTime = DateTime.UtcNow;
            Name = roomTemplate.Name;
            MaxPlayers = roomTemplate.MaxPlayers;
            Locked = roomTemplate.Locked;
            RoomCode = roomCode;
            Tags = new BList<string>();
            TaggedClients = new BHashtable();
        }

        public Room(RoomTemplate roomTemplate, NetworkIdentity owner, string roomCode = null) :
            this(roomTemplate, (INetworkedItem)owner, roomCode)
        {
            OwnerDisplayName = owner.Name;
        }

        public List<string> GetClientsInRandomOrder()
        {
            List<string> c = ConnectedClients.SafeCopy();
            c.Shuffle();
            return c;
        }

        /// <summary>
        /// Joins your client to this room.
        /// </summary>
        /// <param name="client">Your client object.</param>
        /// <param name="joined">The delegate that will get called after you joined the room</param>
        /// <param name="roomPassword">The password to unlock the room if the room is locked.</param>
        public void Join(Client client, ServerResponse<Room> joined, string roomPassword = null)
        {
            RequestPacket packet = client.GetNewRequestPacket(Requests.RequestJoinRoom, (room) =>
            {
                RequestPacket.RequestPacketStatus(room, joined);
            });

            packet.Data.Add("RoomID", ID);
            if (!string.IsNullOrEmpty(roomPassword))
                packet.Data.Add("RoomPassword", roomPassword);

            client.SendToServer(packet);
        }

        /// <summary>
        /// Leaves your client from this room.
        /// Same as <see cref="LeaveCurrent(Client, ServerResponse{Room})"/>, leaves the current joined room.<br />
        /// This is to avoid dirty code with the <see cref="LeaveCurrent(Client, ServerResponse{Room})"/> alternative.
        /// </summary>
        /// <param name="client">Your client object.</param>
        /// <param name="left">The delegate that will get called when done leaving.</param>
        public void Leave(Client client, ServerResponse<Room> left)
        {
            LeaveCurrent(client, left);
        }

        /// <summary>
        /// Same as <see cref="Leave(Client, ServerResponse{Room})"/>, leaves the current joined room.
        /// This approach is static.
        /// </summary>
        /// <param name="client">Your client object.</param>
        /// <param name="left">The delegate that will get called when done leaving.</param>
        public static void LeaveCurrent(Client client, ServerResponse<Room> left)
        {
            RequestPacket packet = client.GetNewRequestPacket(Requests.RequestLeaveRoom, (room) =>
            {
                RequestPacket.RequestPacketStatus(room, left);
            });

            client.SendToServer(packet);
        }

        public bool Contains(string clientID)
        {
            return ConnectedClients.Contains(clientID);
        }

        /// <summary>
        /// Kick certain players from a room, only the owner of the room can do this.
        /// </summary>
        /// <param name="client">Your client object.</param>
        /// <param name="gotKicked">The delegate that will get called when done kicking.</param>
        /// <param name="clientsToKick">The clients(IDs) to kick, leave empty to kick everyone and destroy the room.</param>
        public void Kick(Client client, ServerResponse<Room> gotKicked, params string[] clientsToKick)
        {
            RequestPacket packet = client.GetNewRequestPacket(Requests.RequestKickFromRoom, (room) =>
            {
                RequestPacket.RequestPacketStatus(room, gotKicked);
            });

            packet.Data.Add("RoomID", ID);
            packet.Data.Add("ToKick", clientsToKick);

            client.SendToServer(packet);
        }

        /// <summary>
        /// Broadcasts data to clients in this room.
        /// </summary>
        /// <param name="client">Your client object.</param>
        /// <param name="data">The data to send.</param>
        /// <param name="broadcasted">The delegate that will get called when done broadcasting.</param>
        /// <param name="receivers"></param>
        public void BroadcastInRoom(Client client, Hashtable data, ServerResponse<string> broadcasted, params string[] receivers)
        {
            RequestPacket packet = client.GetNewRequestPacket(Requests.RequestBroadcastInRoom, (room) =>
            {
                RequestPacket.RequestPacketStatus(room, broadcasted);
            });

            packet.Data = new BHashtable(data);
            if (receivers != null && receivers.Length > 0)
                packet.Data.Add("Receivers", receivers);

            client.SendToServer(packet);
        }

        /// <summary>
        /// Destroys the room and kicks all the players in it, only the owner of the room can do this.
        /// </summary>
        /// <param name="client">Your client object.</param>
        /// <param name="gotDestroyed">The delegate that will get called when done destroying.</param>
        public void DestroyRoom(Client client, ServerResponse<Room> gotDestroyed)
        {
            Kick(client, gotDestroyed);
        }

        public bool IsOwner(string clientID)
        {
            return OwnerClientID == clientID;
        }

        public List<string> GetClientsWithTag(string tag)
        {
            if (TaggedClients == null || TaggedClients.Count <= 0)
                return new List<string>();
            else if (TaggedClients.ContainsKey(tag))
                return TaggedClients[tag] as List<string>;
            else
                return new List<string>();
        }

        public List<string> GetClientsNotWithTag(string tag)
        {
            if (TaggedClients == null || TaggedClients.Count <= 0)
                return new List<string>();
            else
            {
                List<string> c = new List<string>();

                foreach (var key in TaggedClients.Keys)
                    if (key.ToString() != tag && TaggedClients[key] is List<string>)
                        c.Union(TaggedClients[key] as List<string>);

                return c;
            }
        }

        public void TagClient(string clientID, string tag)
        {
            if (TaggedClients == null)
                TaggedClients = new BHashtable();

            if (!TaggedClients.ContainsKey(tag))
                TaggedClients.Add(tag, new List<string>());

            List<string> b = TaggedClients[tag] as List<string>;
            if (b != null)
            {
                if (b.Contains(clientID))
                    return;
                else
                    b.Add(clientID);
            }
        }

        public void UnTagClient(string clientID, string tagToRemove)
        {
            if (TaggedClients == null)
                return;

            if (!TaggedClients.ContainsKey(tagToRemove))
                return;

            List<string> b = TaggedClients[tagToRemove] as List<string>;
            if (b != null)
            {
                if (b.Contains(clientID))
                    b.Remove(clientID);
            }
        }

        public void UnTagClients(string tagToRemoveAll)
        {
            if (TaggedClients == null)
                return;

            if (!TaggedClients.ContainsKey(tagToRemoveAll))
                return;

            TaggedClients.Remove(tagToRemoveAll);
        }

        public bool IsClientTaggedWith(string clientID, string withTag)
        {
            if (TaggedClients == null)
                return false;

            if (!TaggedClients.ContainsKey(withTag))
                return false;

            List<string> b = TaggedClients[withTag] as List<string>;
            if (b != null)
                return b.Contains(clientID);
            else
                return false;
        }

        public void TagRoom(string tag)
        {
            if (Tags.Contains(tag))
                return;

            Tags.Add(tag);
        }

        public void ClearRoomTags()
        {
            Tags.Clear();
        }

        public void UnTagRoom(string key)
        {
            Tags.Remove(key);
        }

        public bool IsRoomTaggedWith(string tag)
        {
            return Tags.Contains(tag);
        }

        public override string ToString()
        {
            return $"{ ID } ({ Name } #{ RoomCode }, { PlayerCount }/{ MaxPlayers }{ (Locked ? ", LOCKED" : "") }, { State.ToString() }, Owner: { OwnerClientID })";
        }
    }
}
