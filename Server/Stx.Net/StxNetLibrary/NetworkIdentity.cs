using Stx.Logging;
using Stx.Serialization;
using System;

namespace Stx.Net
{
    /// <summary>
    /// This class provides information about a client.
    /// You should know you can not modify a lot of items in this class because a lot is used server-side.
    /// </summary>
    public class NetworkIdentity : IByteDefined<NetworkIdentity>, INetworkedItem
    {
        /// <summary>
        /// The raw name of this client. When setting outside of the server, make sure you send it back to the server.
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// The current status of this client. This field is read-only, modified only by the server.
        /// </summary>
        public ClientStatus Status { get; protected set; } = ClientStatus.Offline;
        /// <summary>
        /// The exact date and time from when this client was registered. This field is read-only.
        /// </summary>
        public DateTime RegisteredDateTime { get; protected set; }
        /// <summary>
        /// The unique identifier of this client, used to identify itself on the network. This field is read-only, modified only by the server.
        /// </summary>
        public string NetworkID { get; protected set; } = UnknownID;
        /// <summary>
        /// Is this client online?
        /// </summary>
        [DoNotSerialize]
        public bool IsOnline
        {
            get
            {
                return Status != ClientStatus.Offline;
            }
        }

        [DoNotSerialize]
        public ILogger Logger { get; set; } = StxNet.DefaultLogger;

        /// <summary>
        /// The unique identifier of a client that hasn't yet been acknowledged.
        /// </summary>
        public const string UnknownID = "WHO_IS_DIS";

        public NetworkIdentity() : this(UnknownID)
        { }

        public NetworkIdentity(string clientID = UnknownID)
        {
            RegisteredDateTime = DateTime.UtcNow;
            NetworkID = clientID;
        }

        /// <summary>
        /// Server-size only.
        /// Sets <see cref="NetworkID"/> of this <see cref="NetworkIdentity"/> object. Only possible if the current <see cref="NetworkID"/> is equal to <see cref="UnknownID"/>.
        /// </summary>
        /// <param name="clientID"></param>
        public void SetClientID(string clientID)
        {
            if (NetworkID == UnknownID)
                this.NetworkID = clientID;
            else
                Logger.Log("Cannot set ClientID for the second time.", LoggedImportance.CriticalWarning);
        }

        /*public static bool operator==(NetworkIdentity left, NetworkIdentity right)
        {
            return left.ClientID == right.ClientID;
        }

        public static bool operator!=(NetworkIdentity left, NetworkIdentity right)
        {
            return left.ClientID != right.ClientID;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is NetworkIdentity))
            {
                return false;
            }

            return (NetworkIdentity)obj == this;
        }*/


        #region Property adjusters

        /// <summary>
        /// Server-size only.
        /// Adjusts this object's fields as it would be online.
        /// </summary>
        public void CameOnline()
        {
            Status = ClientStatus.PreLobby;
        }

        /// <summary>
        /// Server-size only.
        /// Adjusts this object's fields as it would be offline.
        /// </summary>
        public virtual void CameOffline()
        {
            Status = ClientStatus.Offline;
        }

        #endregion


        public static implicit operator string(NetworkIdentity identity)
        {
            return identity.NetworkID;
        }

        public override string ToString()
        {
            return $"{ NetworkID } (Name: { Name }, Status: { Status }, Online: { IsOnline })";
        }

        public Packet GetNewPacket()
        {
            return new Packet(NetworkID);
        }

        public RequestPacket GetNewRequestPacket(string requestItemName, RequestPacket.RequestResponseDelegate requestResponse)
        {
            return new RequestPacket(NetworkID, requestItemName, requestResponse);
        }
    }
}
