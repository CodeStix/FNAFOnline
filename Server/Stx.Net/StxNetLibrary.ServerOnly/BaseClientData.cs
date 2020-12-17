using System;
using System.IO;
using System.Net.Sockets;

using Stx.Utilities;
using System.Net;
using System.Threading.Tasks;
using Stx.Logging;
using Stx.Net.ServerOnly.RoomBased;

namespace Stx.Net.ServerOnly
{
    public interface IBaseClientData
    {
        Socket Socket { get; }
        IPEndPoint RemoteIP { get; }
        Task PingerTask { get; }
        string NetworkID { get; }
        string Name { get; set; }
         bool IsTimedOut { get; }
        double LastReceiveInterval { get; }
        DateTime LastPingSend { get; set; }
        DateTime LastPingReceived { get; set; }
        float MaxTimeoutSeconds { get; set; }
        int MaxReceivesPerToken { get; set; }
        int ReceiveToken { get; }
        float PingIntervalSeconds { get; set; }
        int CurrentTokenReceives { get; }
        bool Connected { get; }
        bool FirstTimeConnect { get; }

        string ToIdentifiedString();
    }

    /// <summary>
    /// The base class for storing information about a client's connection to the server.
    /// </summary>
    /// <typeparam name="TIdentity">The type to use to store data about the client in. Type must inherit <see cref="NetworkIdentity"/>.</typeparam>
    public class BaseClientData<TIdentity> : IBaseClientData where TIdentity : NetworkIdentity, new()
    {
        public virtual TIdentity Identity { get; set; }
        public Socket Socket { get; }
        public IPEndPoint RemoteIP { get; }
        public Task PingerTask { get; }

        /// <summary>
        /// The unique identifier of this client, used to identify itself on the network.
        /// </summary>
        public string NetworkID
        {
            get
            {
                return Identity.NetworkID;
            }
            set
            {
                Identity.SetClientID(value);
            }
        }
        /// <summary>
        /// The actual name of this client.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return Identity.Name;
            }
            set
            {
                Identity.Name = value;
            }
        }
        public bool IsTimedOut
        {
            get
            {
                return (LastPingSend - LastPingReceived).TotalSeconds >= MaxTimeoutSeconds;
            }
        }
        public double LastReceiveInterval
        {
            get
            {
                return (LastPingSend - LastPingReceived).TotalSeconds;
            }
        }
        public DateTime LastPingSend { get; set; }
        public DateTime LastPingReceived { get; set; }
        public float MaxTimeoutSeconds { get; set; } = 30f;
        public int MaxReceivesPerToken { get; set; } = 10;
        public int ReceiveToken { get; internal set; } = 0;
        public float PingIntervalSeconds { get; set; } = 4.5f;
        public int CurrentTokenReceives { get; private set; } = 0;
        public bool Connected { get; internal set; } = false;
        public bool FirstTimeConnect { get; private set; } = false;

        public Action<BaseClientData<TIdentity>> TimedOutAction { get; set; }

        public ILogger Logger { get; set; } = StxNet.DefaultLogger;

        public BaseClientData(Socket socket)
        {
            Socket = socket;
            RemoteIP = Socket.RemoteEndPoint as IPEndPoint;
            PingerTask = PingTask();
            LastPingSend = DateTime.UtcNow;
            LastPingReceived = DateTime.UtcNow;
            Identity = new TIdentity();

            //if (tryLoadFromDisk)
            //    Identity = ClientRegisterer<TIdentity>.LoadIdentityFor(NetworkID);
            //else
            //    Identity = new TIdentity();
        }

        private async Task PingTask()
        {
            do
            {
                await Task.Delay((int)(1000 * PingIntervalSeconds));

                Ping();
            }
            while (Socket.Connected);
        }

        public void Ping()
        {
            LastPingSend = DateTime.UtcNow;

            Server.Send(this, new byte[1], BytesContentType.Ping);
            
            if (IsTimedOut)
            {
                TimedOutAction?.Invoke(this);
            }
        }

        /// <summary>
        /// Loads this clients identity (<see cref="Identity"/>) from disk using <see cref="ClientRegisterer{TIdentity}"/>. 
        /// <param name="asClientID">The id of the identity to load into this client data object.</param>
        /// </summary>
        public void Load(string asClientID)
        {
            if (asClientID != NetworkIdentity.UnknownID)
            {
                //Logger.Debug("Loading client data for " + ToIdentifiedString());

                FirstTimeConnect = !ClientRegisterer<TIdentity>.IdentityExistsFor(asClientID);
                Identity = ClientRegisterer<TIdentity>.LoadIdentityFor(asClientID);
            }
        }

        /// <summary>
        /// Saves this clients identity (<see cref="Identity"/>) to disk using <see cref="ClientRegisterer{TIdentity}"/>. 
        /// </summary>
        public void Save()
        {
            if (Identity.NetworkID != NetworkIdentity.UnknownID)
            {
                //Logger.Debug("Saving client data for " + ToIdentifiedString());

                ClientRegisterer<TIdentity>.SaveIdentityFor(Identity);
            }
        }

        internal bool Authorize(string token)
        {
            string fullPath = ClientRegisterer<TIdentity>.GetFullAuthSavePath(NetworkID);

            if (!StringChecker.IsValidPassword(token))
                return false;

            if (!File.Exists(fullPath))
            {
                DirectoryInfo fi = new DirectoryInfo(ClientRegisterer<TIdentity>.DataSaveLocation);

                if (!fi.Exists)
                    fi.Create();

                File.WriteAllText(fullPath, token);
                return true;
            }
            else
            {
                return File.ReadAllText(fullPath) == token;
            }
        }

        public virtual void DisconnectMe(DisconnectReason reason)
        {
            Connected = false;

            Identity.CameOffline();

            Save();
            
            if (reason != DisconnectReason.Unknown && reason != DisconnectReason.WentOfflineIntended)
                Server.Send(Socket, new byte[] { (byte)reason }, BytesContentType.DisconnectReason);

            try
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
            }
            catch
            { }
        }

        public override string ToString()
        {
            return $"{ NetworkID } ({ Identity?.ToString() })";
        }

        public string ToIdentifiedString()
        {
            return $"{ NetworkID } ({ Identity.Name }; { RemoteIP?.ToString() })";
        }

        public bool CanReceive(int forToken)
        {
            if (forToken != ReceiveToken)
            {
                CurrentTokenReceives = 0;
                ReceiveToken = forToken;
            }

            return CurrentTokenReceives++ <= MaxReceivesPerToken;
        }
    }
}
