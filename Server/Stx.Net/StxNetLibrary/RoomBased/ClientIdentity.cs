using Stx.Serialization;
using Stx.Utilities;

namespace Stx.Net.RoomBased
{
    /// <summary>
    /// This class provides a lot of information about clients and their matchmaking activities, you should know you can not modify a lot of items in this class because a lot is used server-side.
    /// </summary>
    public class ClientIdentity : NetworkIdentity, IByteDefined<ClientIdentity>
    {
        /// <summary>
        /// This field is modifiable, make sure you send it back to the server after modifying.
        /// </summary>
        public ClientInfo Info { get; set; }
        /// <summary>
        /// This field is read-only, modified only by the server.
        /// null if this client is currently not in a room.
        /// </summary>
        public string RoomID { get; private set; } = null;
        /// <summary>
        /// The current status of this client in his/her current room.
        /// </summary>
        public ClientRoomStatus RoomStatus { get; set; } = ClientRoomStatus.NotReady;
        /// <summary>
        /// Their actual name, no modifications were made.
        /// </summary>
        [DoNotSerialize]
        public override string Name
        {
            get
            {
                return Info.Name;
            }
            set
            {
                Info.Name = value;
            }
        }
        /// <summary>
        /// The name that is used to display their identity in UI, modified only if they have the default name.
        /// </summary>
        [DoNotSerialize]
        public string DisplayName
        {
            get
            {
                return Info.Name == ClientInfo.DefaultName ? SecretDisplayName : Info.Name;
            }
        }
        /// <summary>
        /// The name that is used to display their identity in UI, with a max length of 16 letters, without showing who it actually is. (ex: Player589)
        /// </summary>
        [DoNotSerialize]
        public string SecretDisplayName
        {
            get
            {
                return StringUtil.Shorten(NetworkID, 16, "Player");
            }
        }
        /// <summary>
        /// The name that is used to display their identity in UI, with a max length of 16 letters.
        /// </summary>
        [DoNotSerialize]
        public string ShortDisplayName
        {
            get
            {
                return StringUtil.ShortenIfNeeded(DisplayName, 16, "", "...");
            }
        }
        /// <summary>
        /// Checks if this client is in a room.
        /// </summary>
        /// <returns>The fact that the client is in a room.</returns>
        [DoNotSerialize]
        public bool IsInAnyRoom
        {
            get
            {
                return !string.IsNullOrEmpty(RoomID);
            }
        }

        public ClientIdentity() : base()
        {
            this.Info = new ClientInfo();
        }

        public ClientIdentity(string clientID = UnknownID) : base(clientID)
        {
            this.Info = new ClientInfo();
        }

        /// <summary>
        /// Server-size only.
        /// Adjusts this object's fields as it would be offline.
        /// </summary>
        public override void CameOffline()
        {
            base.CameOffline();

            RoomID = null;
            RoomStatus = ClientRoomStatus.NotReady;
        }

        /// <summary>
        /// Server-size only.
        /// Adjusts this object's fields as it would be in no room.
        /// </summary>
        public void CamePreLobby()
        {
            RoomID = null;
            Status = ClientStatus.PreLobby;
            RoomStatus = ClientRoomStatus.NotReady;
        }

        /// <summary>
        /// Checks if this client is in the specified room.
        /// </summary>
        /// <param name="roomID">The id of the room to check.</param>
        /// <returns>The fact that it is in that room.</returns>
        public bool IsInRoom(string roomID)
        {
            return RoomID == roomID;
        }

        /// <summary>
        /// Checks if this client is in the specified room.
        /// </summary>
        /// <param name="room">The room to check.</param>
        /// <returns>The fact that it is in that room.</returns>
        public bool IsInRoom(Room room)
        {
            return RoomID == room.ID;
        }

        /// <summary>
        /// Server-size only.
        /// Adjusts this object's fields as it would be in a room.
        /// </summary>
        public void CameInRoom(Room room)
        {
            CameInRoom(room.ID);
        }

        /// <summary>
        /// Server-size only.
        /// Adjusts this object's fields as it would be in a room.
        /// </summary>
        public void CameInRoom(string roomID)
        {
            RoomID = roomID;
            Status = ClientStatus.InLobby;

            RoomStatus = ClientRoomStatus.NotReady;
        }

        /// <summary>
        /// Server-size only.
        /// Adjusts this object's fields as it would be in-game.
        /// </summary>
        public void CameInGame()
        {
            if (Status == ClientStatus.InLobby)
                Status = ClientStatus.InGame;

            RoomStatus = ClientRoomStatus.NotReady;
        }

        /// <summary>
        /// Server-size only.
        /// Adjusts this object's fields as it would be back in the lobby after being in a game.
        /// </summary>
        public void CameBackLobby()
        {
            if (Status == ClientStatus.InGame)
                Status = ClientStatus.InLobby;

            RoomStatus = ClientRoomStatus.NotReady;
        }

        public override string ToString()
        {
            return $"{ NetworkID } (Status: { Status }, CurrentRoom: { RoomID }, Registered: { RegisteredDateTime.ToLongDateString() }, Info: { Info }) ";
        }
    }
}
