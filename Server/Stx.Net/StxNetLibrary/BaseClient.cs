using Stx.Logging;
using Stx.Serialization;
using Stx.Utilities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Stx.Net
{
    public delegate void PacketCompleterDelegate(RequestPacket forPacket, string requiredKey, Type requiredKeyType, Action<bool> submit);

    public class BaseClient<TIdentity> : ThreadSafeDataTransfer<Packet>, IBaseClient<TIdentity>, IServerConnected, INetworkedItem where TIdentity : NetworkIdentity, new()
    {
        /// <summary>
        /// Your unique identifier on the network.
        /// </summary>
        public string NetworkID { get; private set; }
        /// <summary>
        /// The endpoint this client will get connected too.
        /// </summary>
        public IPEndPoint Remote { get; }
        /// <summary>
        /// The <see cref="Stx.Net.DataReceiver"/> of this client, add your data handling here.
        /// </summary>
        public DataReceiver DataReceiver { get; set; }
        /// <summary>
        /// The task scheduler of this client. See <see cref="ThreadSafeData"/> to invoke tasks safely.
        /// </summary>
        public TaskScheduler Scheduler { get; set; } = new TaskScheduler();
        /// <summary>
        /// Are we connected to the server?
        /// </summary>
        public bool Connected { get; private set; } = false;
        /// <summary>
        /// Your 'password' to authorize on the server with your client ID.
        /// </summary>
        public string ClientAuthorisationToken { get; set; }
        /// <summary>
        /// The key of the application this client is running on.
        /// </summary>
        public string ApplicationKey { get; }
        /// <summary>
        /// The name of the application this client is running on.
        /// </summary>
        public string ApplicationName { get; set; } = StxNet.DefaultApplicationName;
        /// <summary>
        /// The version of the application this client is running on.
        /// </summary>
        public string ApplicationVersion { get; set; } = StxNet.DefaultApplicationVersion;
        /// <summary>
        /// A read-only version of your identity on the server, downloaded when the connection is established.
        /// </summary>
        public TIdentity You { get; private set; }
        /// <summary>
        /// The reason you got disconnected lately.
        /// </summary>
        public DisconnectReason? DisconnectReason { get; private set; } = null;
        /// <summary>
        /// The URL you got from the server if your client version is outdated. Get the new version from this location.
        /// </summary>
        public string UpToDateApplicationUrl { get; private set; } = null;
        /// <summary>
        /// The size of the receive buffer, must be the same as on the server to avoid problems. Default: 16384 bytes.
        /// </summary>
        public int ReceiveBufferSize { get; set; } = StxNet.DefaultReceiveBufferSize;
        /// <summary>
        /// Do you want to ignore incoming server pings? This will lead to timeout on server. Testing purposes only.
        /// </summary>
        public bool IgnorePings { get; set; } = false;
        /// <summary>
        /// The underlaying <see cref="System.Net.Sockets.Socket"/> of this client.
        /// </summary>
        public Socket Socket { get; }

        private string clientNameToSend;
        private bool connectAsync = false;

        /// <summary>
        /// Gets called when this client receives a packet from the server. Take a look at <see cref="ThreadSafeData"/> to make this event thread-safe. 
        /// </summary>
        /// <param name="p">The received packet.</param>
        public delegate void ReceiveDelegate(Packet p);
        public event ReceiveDelegate OnReceived;

        /// <summary>
        /// Gets called when this client establishes a connection with the server. Take a look at <see cref="ThreadSafeData"/> to make this event thread-safe. 
        /// </summary>
        /// <param name="firstTime">Is this the first time this client connects to the server?</param>
        public delegate void ConnectedDelegate(bool firstTime);
        public event ConnectedDelegate OnConnected;

        /// <summary>
        /// Gets called when this client loses connection with the server.
        /// </summary>
        public delegate void DisconnectedDelegate(DisconnectReason? reason);
        public event DisconnectedDelegate OnDisconnected;

        /// <summary>
        /// Gets called when this client requires a software update. Take a look at <see cref="ThreadSafeData"/> to make this event thread-safe. 
        /// </summary>
        /// <param name="updateDownloadLocation">The location where you can download the update, specified by the server.</param>
        public delegate void UpdateRequiredDelegate(string updateDownloadLocation);
        public event UpdateRequiredDelegate OnUpdateRequired;

        /// <summary>
        /// Gets called when this client receives a server announcement (ex. "downtime in 20 minutes"). Take a look at <see cref="ThreadSafeData"/> to make this event thread-safe. 
        /// </summary>
        /// <param name="message">The announced message.</param>
        public delegate void AnnouncementDelegate(string message);
        public event AnnouncementDelegate OnAnnouncement;

        /// <summary>
        /// Gets called when this cannot connect to the server.
        /// </summary>
        /// <param name="ex">The internal exception why we couldn't connect.</param>
        public delegate void CannotConnectDelegate(Exception ex);
        public event CannotConnectDelegate OnCannotConnect;

        /// <summary>
        /// Gets called when a request was send on the server, but the server did not receive the required data to process the request. 
        /// Use this delegate to complete the required data for the specified packet.
        /// <para>Invoke submit with true to send this packet back to the server, or submit false to mark this request as failed.</para>
        /// </summary>
        /// <param name="forPacket">The packet that stills requires data.</param>
        /// <param name="requiredKey">The required key the packet is missing.</param>
        /// <param name="requiredKeyType">The required type of the value that is associated with the key.</param>
        /// <param name="submit">The submit delegate used to send this packet back to the server after modifying the packet or mark it as failed.</param>
        /// <returns>True to send back to the server, false to mark this request as failed.</returns>
        public PacketCompleterDelegate PacketCompleter { get; set; }

        public ILogger Logger { get; set; } = StxNet.DefaultLogger;

        public BaseClient(ConnectionStartInfo connectInfo)
        {
            if (!connectInfo.IsValid())
            {
                Logger.Log("ConnectionStartInfo is not valid, please tweak its settings to match the requirements!", LoggedImportance.CriticalError);
                return;
            }

            Remote = connectInfo.EndPoint;
            NetworkID = connectInfo.ClientID;
            ApplicationKey = connectInfo.ApplicationKey;
            ApplicationName = connectInfo.ApplicationName;
            ApplicationVersion = connectInfo.ApplicationVersion;
            ClientAuthorisationToken = connectInfo.AuthorizationToken;
            clientNameToSend = connectInfo.ClientName;
            connectAsync = connectInfo.ConnectAsync;

            if (StxNet.IsServer)
                return;
            StxNet.IsClient = true;
            StxNet.IsServer = false;

            RegisterNetworkTypes();

            Socket = new Socket(Remote.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.NoDelay = true;

            if (ReceiveBufferSize <= 0)
                ReceiveBufferSize = StxNet.DefaultReceiveBufferSize;

            DataReceiver = new DataReceiver();
            RegisterDataHandlers(DataReceiver);

            if (connectInfo.ConnectOnConstruct)
                Connect();
        }

        /// <summary>
        /// Create this object to establish a server connection.
        /// </summary>
        /// <param name="applicationKey">The key of the application this client is used for. Must be the same on the server.</param>
        /// <param name="connectAddress">The remote address of the server.</param>
        /// <param name="port">The port on the server.</param>
        /// <param name="authToken">Your unique authorization token, its is your password to your identity on the server, every ClientID has a authorisation token, the first time you connect is the only time you can choose it.</param>
        /// <param name="clientID">Your client ID. Acquire a new one using <see cref="Guid.NewGuid()"/>.</param>
        public BaseClient(string applicationKey, string host, ushort port, string authToken, string clientID)
            : this(new ConnectionStartInfo(host, port, applicationKey, false) { ClientID = clientID, AuthorizationToken = authToken })
        { }

        /// <summary>
        /// Use this method to connect (or reconnect) synchronously or asynchronously (predefined, via <see cref="ConnectionStartInfo"/>) to the server.
        /// </summary>
        public void Connect()
        {
            if (!connectAsync)
                ConnectSync();
            else
                ConnectAsync();
        }

        /// <summary>
        /// Use this method to synchronously connect (or reconnect) to the server.
        /// </summary>
        public virtual void ConnectSync()
        {
            if (Connected)
                return;

            try
            {
                Socket.Connect(Remote);

                BeginReceive();

                // Send acknowledge so the server knows our ClientID.
                SendToServer(GetAuthorizeBytes(), BytesContentType.AuthorizationScheme);
            }
            catch (Exception e)
            {
                Logger.LogException(e, "Could not connect");

                ThreadSafeData.Invoke(() =>
                {
                    OnCannotConnect?.Invoke(e);
                });

                Connected = false;
            }
        }

        /// <summary>
        /// Use this method to asynchronously connect (or reconnect) to the server.
        /// </summary>
        public virtual void ConnectAsync()
        {
            if (Connected)
                return;

            try
            {
                Socket.BeginConnect(Remote, new AsyncCallback(ConnectCallback), null);
            }
            catch(Exception e)
            {
                Logger.LogException(e, "Could not connect");

                ThreadSafeData.Invoke(() =>
                {
                    OnCannotConnect?.Invoke(e);
                });

                Connected = false; 
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket.EndConnect(ar);

                /*receiveThread = new Thread(DataReceive);
                receiveThread.Start();*/
                BeginReceive();

                // Send acknowledge so the server knows our ClientID and other stuff...
                SendToServer(GetAuthorizeBytes(), BytesContentType.AuthorizationScheme);
            }
            catch (Exception e)
            {
                Logger.LogException(e, "Could not connect");

                ThreadSafeData.Invoke(() =>
                {
                    OnCannotConnect?.Invoke(e);
                });

                Connected = false;
            }
        }

        private byte[] GetAuthorizeBytes()
        {
            const long State = 9896;

            XorShift.Initiate(State);

            Stack<byte[]> bs = new Stack<byte[]>();
            bs.Push(ByteUtil.Xor(Encoding.ASCII.GetBytes(StxNet.ClientStxVersion), XorShift.NextByte()));
            bs.Push(ByteUtil.Xor(Encoding.ASCII.GetBytes(ApplicationName), XorShift.NextByte()));
            bs.Push(ByteUtil.Xor(Encoding.ASCII.GetBytes(ApplicationVersion), XorShift.NextByte()));
            bs.Push(ByteUtil.Xor(Encoding.ASCII.GetBytes(ApplicationKey), XorShift.NextByte()));
            bs.Push(ByteUtil.Xor(Encoding.ASCII.GetBytes(NetworkID), XorShift.NextByte()));
            bs.Push(ByteUtil.Xor(Encoding.ASCII.GetBytes(ClientAuthorisationToken), XorShift.NextByte()));
            if (!string.IsNullOrEmpty(clientNameToSend))
                bs.Push(ByteUtil.Xor(Encoding.ASCII.GetBytes(clientNameToSend), XorShift.NextByte()));
            return ByteUtil.FromSegmentStack(bs);
        }
    
        public virtual void ProcessAcknowledgmentPacket(Packet p, bool isFirstTimeConnecting)
        {
            if (p.Data.Requires<TIdentity>("You"))
                You = (TIdentity)p.Data["You"];

            OnConnected?.Invoke(isFirstTimeConnecting);
        }

        ~BaseClient()
        {
            if (Connected)
                StopConnection();
        }

        /// <summary>
        /// This method registers all the default types that will be send over network, to include your own, override this method.
        /// <para>See <see cref="Bytifier.Include{T}"/>. Make sure you include all the types in the same order at the other side!</para>
        /// </summary>
        public virtual void RegisterNetworkTypes()
        {
            StxNet.RegisterNetworkTypes();
        }

        /// <summary>
        /// This method registers all the data-handlers associated with data keys in packets. To register your own <see cref="DataHandler{T}"/>, or override the default ones,
        /// override this method and use <see cref="DataReceiver.AddHandler(AnyDataHandler)"/> to add your own data handler.
        /// </summary>
        /// <param name="receiver">The data receiver to add data-handlers too.</param>
        public virtual void RegisterDataHandlers(DataReceiver receiver)
        {
            receiver.AddHandler(new DataHandler<bool>("_FirstTime", (b, p) =>
            {
                if (!Connected)
                {
                    ProcessAcknowledgmentPacket(p as Packet, b);

                    Connected = true;
                }
            }));
            receiver.AddHandler(new DataHandler<string>("_Announcement", (a, p) =>
            {
                LocalAnnounce(a);
            }));
            receiver.AddHandler(new ObjectHandler("_Disconnect", (a, p) =>
            {
                StopConnection();
            }));
        }

        public void LocalAnnounce(string message)
        {
            OnAnnouncement?.Invoke(message);
        }

        public Packet GetNewPacket()
        {
            return new Packet(NetworkID);
        }

        public RequestPacket GetNewRequestPacket(string requestItemName, RequestPacket.RequestResponseDelegate requestResponse)
        {
            return new RequestPacket(NetworkID, requestItemName, requestResponse);
        }

        public void SendToServer(Packet p)
            => SendToServer(p.ToBytes(), BytesContentType.Packet);

        public void SendToServer(byte[] buffer, BytesContentType contentType = BytesContentType.Packet)
        {
            if (buffer == null || buffer.Length <= 0 || !Socket.Connected)
                return;

            buffer = ByteWrapper.Wrap(buffer, contentType);
            buffer = ByteUtil.WrapSegmentedBytes(buffer);

            try
            {
                Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallback, null);
            }
            catch(Exception e)
            {
                Logger.LogException(e, "Could not send data to server");
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket.EndSend(ar);
            }
            catch(Exception e)
            {
                Logger.LogException(e, "Could not send data to server via callback");
            }
        }



        /*public void RequestDisconnect()
        {
            RequestPacket rp = new RequestPacket(ClientID, Identifiers.REQUEST_DISCONNECT, (e) => 
            {
                Connected = false;
            });

            SendToServer(rp.ToBytes());
        }*/

        public void Disconnect()
        {
            DisconnectReason = Net.DisconnectReason.WentOfflineIntended;

            StopConnection();
        }

        public void StopConnection()
        {
            if (!Connected)
                return;

            Connected = false;

            try
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
            }
            catch
            {}

            /*try
            {
                receiveThread.Abort();
            }
            catch
            { }*/
        }

        class StateObject
        {
            public byte[] buffer;
        }

        public void BeginReceive()
        {
            StateObject state = new StateObject();
            state.buffer = new byte[ReceiveBufferSize];

            Socket.BeginReceive(state.buffer, 0, ReceiveBufferSize, 0, ReadCallback, state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;

            // Read data from the socket.   
            int sizeReceived = 0;
            try
            {
                sizeReceived = Socket.EndReceive(ar);

                foreach (byte[] b in ByteUtil.UnwrapSegmentedBytes(state.buffer))
                   ProcessBytes(b);

                Array.Clear(state.buffer, 0, state.buffer.Length);

                Socket.BeginReceive(state.buffer, 0, ReceiveBufferSize, 0, ReadCallback, state);
            }
            catch
            {
                Connected = false;

                ThreadSafeData.Invoke(() =>
                {
                    OnDisconnected?.Invoke(DisconnectReason);
                });

                StopConnection();
            }
        }

        public void ProcessBytes(byte[] buffer)
        {
            ByteWrapper bw = ByteWrapper.UnWrap(buffer);

           // DebugHandler.Info($"Received { bw.DataBuffer.Length } from the server.");

            if (bw.DataBuffer.Length > 0)
            {
                switch(bw.ContentType)
                {
                    case BytesContentType.Ping:
                        if (!IgnorePings)
                            SendToServer(new byte[1], BytesContentType.Ping);
                        break;

                    case BytesContentType.DisconnectReason:
                        DisconnectReason = (DisconnectReason)bw.DataBuffer[0];
                        break;

                    case BytesContentType.UpdateLocation:
                        UpToDateApplicationUrl = Encoding.ASCII.GetString(bw.DataBuffer, 0, bw.DataBuffer.Length);
                        ThreadSafeData.Invoke(() =>
                        {
                            OnUpdateRequired?.Invoke(UpToDateApplicationUrl);
                        });
                        break;

                    case BytesContentType.Announcement:
                        ThreadSafeData.Invoke(() =>
                        {
                            OnAnnouncement?.Invoke(Encoding.ASCII.GetString(bw.DataBuffer, 0, bw.DataBuffer.Length));
                        });
                        break;

                    case BytesContentType.Packet:
                        Packet p = Bytifier.Object<Packet>(bw.DataBuffer);//Packet.AnyPacketFromBytes(buffer);

                        if (p is RequestPacket)
                        {
                            RequestPacket rp = (RequestPacket)p;

                            if (rp.DidRespond())
                            {
                                Transfer(rp);
                            }
                            else
                            {
                                //We have to respond to packet.
                                /*if (rp.RequestItemName == "Ping")
                                {
                                    rp.ResponseOk();
                                    rp.SetResponderID(NetworkID);
                                    Socket.TrySend(rp.ToBytes());
                                }*/
                            }
                        }
                        else
                        {
                            Transfer(p);
                            DataReceiver?.DidReceive(p);
                        }
                        break;
                }
            }
        }

        protected override void Received(Packet data)
        {
            if (data is RequestPacket)
            {
                RequestPacket rp = (RequestPacket)data;

                // The packet was marked as incomplete, we have to complete it.
                if (rp.ResponseObject is RequestPacket.Completion)
                {
                    if (PacketCompleter != null)
                    {
                        RequestPacket.Completion pc = (RequestPacket.Completion)rp.ResponseObject;

                        PacketCompleter.Invoke(rp, pc.RequiredKey, Type.GetType(pc.RequiredKeyTypeName, false, true), (success) =>
                        {
                            if (success)
                            {
                                rp.SetResponderID(NetworkID);
                                rp.ResponseObject = null;
                                SendToServer(rp);
                            }
                            else
                            {
                                rp.ResponseFail();
                                rp.InvokeResponseMethods();
                                OnReceived?.Invoke(data);
                            }
                        });

                        return;
                    }
                    else
                    {
                        rp.ResponseFail();
                    }
                }

                //Our request got responded => invoke method associated with request.
                rp.InvokeResponseMethods();
            }

            OnReceived?.Invoke(data);
        }
    }
}
