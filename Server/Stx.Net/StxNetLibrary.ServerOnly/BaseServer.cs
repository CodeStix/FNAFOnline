using Stx.Collections.Concurrent;
using Stx.Logging;
using Stx.Net.ServerOnly.RoomBased;
using Stx.Serialization;
using Stx.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stx.Net.ServerOnly
{
    public delegate void RequestDelegate<TIdentity>(RequestPacket packet, BaseClientData<TIdentity> sender) where TIdentity : NetworkIdentity, new();

    public class BaseServer<TIdentity> : IBaseServer<TIdentity>, INetworkedItem where TIdentity : NetworkIdentity, new()
    {
        public List<string> BannedClients { get; set; } = new List<string>();
        public List<string> BannedClientIPs { get; set; } = new List<string>();
       
        public Dictionary<string, RequestDelegate<TIdentity>> RequestHandlers { get; } = new Dictionary<string, RequestDelegate<TIdentity>>();
        public TaskScheduler Scheduler { get; } = new TaskScheduler();
        public ServerCommands<TIdentity> CommandProvider { get; }
        public ILogger Logger { get; set; } = StxNet.DefaultLogger;

        protected ConcurrentDictionary<string, BaseClientData<TIdentity>> ConnectedClients { get; } = new ConcurrentDictionary<string, BaseClientData<TIdentity>>();
        protected ConcurrentList<BaseClientData<TIdentity>> UnknownClients { get; } = new ConcurrentList<BaseClientData<TIdentity>>();

        /// <summary>
        /// How many clients are connected to this server?
        /// </summary>
        public int ConnectedCount
        {
            get
            {
                return ConnectedClients.Count;
            }
        }
        /// <summary>
        /// Are clients allowed to connect?
        /// </summary>
        public bool CanConnect { get; set; } = true;
        /// <summary>
        /// A value that gets refreshed every second. Each client can at most send a specified packets(<see cref="MaxReceivesPerSecondPerClient"/>) per token.
        /// </summary>
        public int SendToken { get; private set; } = 0;

        /// <summary>
        /// The key of the application this server is used for.
        /// The client must use a copy of this key to connect to the server!
        /// </summary>
        public string ApplicationKey { get; }
        /// <summary>
        /// The name of the application this server is used for.
        /// </summary>
        public string ApplicationName { get; set; } = StxNet.DefaultApplicationName;
        /// <summary>
        /// The version of the application this server is used for.
        /// </summary>
        public string ApplicationVersion { get; set; } = StxNet.DefaultApplicationVersion;
        /// <summary>
        /// The update URL to send to an outdated client.
        /// </summary>
        public string UpToDateApplicationUrl { get; set; }
        /// <summary>
        /// The maximum player count that can connect to the server at once.
        /// </summary>
        public int MaxConnections { get; set; } = 50;
        /// <summary>
        /// The interval to send ping packets to all the clients.
        /// </summary>
        public float PingIntervalSeconds { get; set; } = 4f;
        /// <summary>
        /// The time before disconnecting a client when it does not reply on a ping packet.
        /// </summary>
        public float MaxTimeoutSeconds { get; set; } = 16f;
        /// <summary>
        /// The ID of the server on the network.
        /// </summary>
        public string NetworkID { get; set; } = "server";
        /// <summary>
        /// The maximum amount of packets each client can send per second.
        /// </summary>
        public int MaxReceivesPerSecondPerClient { get; set; } = 10;
        /// <summary>
        /// The size (in bytes) of the receive/send buffer per client, increase when you are 
        /// sending huge packets or sending a lot of packets in a small amount of time (can flow into the same receive buffer).
        /// You should not touch this if you do not know what you are doing.
        /// </summary>
        public int SendReceiveBufferSizePerClient { get; set; } = StxNet.DefaultReceiveBufferSize;
        /// <summary>
        /// The fact that this server is running.
        /// </summary>
        public bool IsRunning { get; private set; } = false;

        public const string PluginsLocation = @"./Plugins/";
        public const string AchievementFile = @"achievements.json";

        internal Socket listenerSocket;
        internal Thread listenerThread;

        public virtual BaseClientData<TIdentity> GetConnectedClient(string clientID)
        {
            /*BaseClientData<TIdentity> client;
            if (ConnectedClients.TryGetValue(clientID, out client))
                return client;
            else
                return null;*/

            if (!ConnectedClients.ContainsKey(clientID))
                return null;

            return ConnectedClients[clientID];
        }

        public virtual ICollection<BaseClientData<TIdentity>> GetConnectedClients()
        { 
            return ConnectedClients.Values;
        }

        public virtual BaseClientData<TIdentity> GetRandomConnectedClient()
        {
            if (ConnectedCount == 0)
                return null;
            else
                return ConnectedClients[ConnectedClients.Keys.ToArray()[XorShift.NextInt(0, ConnectedCount)]];
        }

        public virtual BaseClientData<TIdentity> GetConnectedClientFromName(string name)
        {
            return ConnectedClients.Values.FirstOrDefault((e) => name == e.Name);
        }

        public BaseServer(string applicationKey, ushort port)
        {
            ApplicationKey = applicationKey;

            if (StxNet.IsClient)
                return;
            StxNet.IsServer = true;

            Logger.Log($"Starting server for application {{{ applicationKey }}} ...");

            CommandProvider = new ServerCommands<TIdentity>(this);

            // Include Bytifier types...
            RegisterNetworkTypes();

            RequestHandlers.Add(Requests.RequestPing, RequestPing);
            RequestHandlers.Add(Requests.RequestDisconnect, RequestDisconnect);
            RequestHandlers.Add(Requests.RequestSetName,  RequestSetName);

            RequestHandlers.Add("Test", RequestTest);

            // Actually hosting the server...
            listenerSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            listenerSocket.ExclusiveAddressUse = true;

            if (SendReceiveBufferSizePerClient <= 0)
                SendReceiveBufferSizePerClient = StxNet.DefaultReceiveBufferSize;

            IPEndPoint local = new IPEndPoint(IPAddress.IPv6Any, port);

            Logger.Log($"Listening for connections on { local }...");

            listenerSocket.Bind(local);

            listenerThread = new Thread(new ThreadStart(ListenerThread));
            listenerThread.Start();

            CheckerTask();

            Logger.Log("Reading client identities...");

            ClientRegisterer<TIdentity>.LoadRegisteredClients();

            Logger.Log("Ready.");

            IsRunning = true;
        }

        /// <summary>
        /// This method registers all the types that will be send over network, to include your own, override this method.
        /// See <see cref="Bytifier.Include{T}"/>. Make sure you include all the types in the same order at the client side!
        /// </summary>
        public virtual void RegisterNetworkTypes()
        {
            StxNet.RegisterNetworkTypes();
        }

        public virtual void Stop()
        {
            Logger.Log($"Stopping...", LoggedImportance.Debug);

            CanConnect = false;

            try
            {
                listenerSocket.Shutdown(SocketShutdown.Both);
                listenerSocket.Close();
            }
            catch { }

            var toDisconnect = new HashSet<BaseClientData<TIdentity>>(ConnectedClients.Values);
            foreach (var data in toDisconnect)
            {
                Logger.Log($"Disconnecting/saving { data.ToIdentifiedString() }...", LoggedImportance.Debug);

                DisconnectClient(data, DisconnectReason.HostShutdown);
            }

            IsRunning = false;
        }

        private async void CheckerTask()
        {
            while (true)
            {
                await Task.Delay(1000);

                SendToken = XorShift.NextInt();
            }
        }

        private ManualResetEvent acceptConnectionDone = new ManualResetEvent(false);

        private void ListenerThread()
        {
            while (true)
            {
                try
                {
                    listenerSocket.Listen(MaxConnections);

                    // Set the event to non-signaled state.  
                    acceptConnectionDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    listenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), listenerSocket);

                    // Wait until a connection is made before continuing.  
                    acceptConnectionDone.WaitOne();
                }
                catch (Exception e)
                {
                    Logger.LogException(e, "Could not accept or listen for connection");
                }
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                // Get the socket that handles the client request.  
                Socket listener = (Socket)ar.AsyncState;
                Socket accepted = listener.EndAccept(ar);
                accepted.NoDelay = true;
                accepted.ReceiveBufferSize = SendReceiveBufferSizePerClient;
                accepted.SendBufferSize = SendReceiveBufferSizePerClient;

                IPEndPoint acceptedEnd = accepted.RemoteEndPoint as IPEndPoint;

                if (!CanConnect || ConnectedCount >= MaxConnections || acceptedEnd == null || BannedClientIPs.Contains(acceptedEnd.Address.ToString()))
                {
                    DisconnectSocket(accepted, DisconnectReason.KickedByHost);
                    return;
                }

                BaseClientData<TIdentity> connected = GetClientDataForAccepted(accepted);
                connected.MaxTimeoutSeconds = MaxTimeoutSeconds;
                connected.MaxReceivesPerToken = MaxReceivesPerSecondPerClient;

                UnknownClients.Add(connected);

                // Create the state object.  
                StateObject state = new StateObject();
                state.isAcknowledge = true;
                state.sender = connected;
                state.buffer = new byte[SendReceiveBufferSizePerClient];
                accepted.BeginReceive(state.buffer, 0, SendReceiveBufferSizePerClient, 0, ReadCallback, state);

                ServerStats.connectionsAccepted++;

                Logger.Log($"Accepted new connection. { connected.ToIdentifiedString() }", LoggedImportance.Debug);
            }
            catch(Exception ex)
            {
                Logger.Log($"Could not accept connection: {ex.Message}", LoggedImportance.CriticalError);
            }
            finally
            {
                acceptConnectionDone.Set();
            }
        }

        class StateObject
        {
            public byte[] buffer;
            public bool isAcknowledge = true;
            public BaseClientData<TIdentity> sender;
        }

        private void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            BaseClientData<TIdentity> sender = state.sender;

            // Read data from the client socket.   
            int sizeReceived = 0;

            try
            {
                sizeReceived = sender.Socket.EndReceive(ar);
            }
            catch
            { }

            if (sizeReceived == 0)
            {
                // Connection is terminated, either by force or willingly.
                DisconnectClient(sender, DisconnectReason.WentOfflineIntended);

                return;
            }

            ServerStats.bytesReceived += sizeReceived;

            if (sender.CanReceive(SendToken))
            {
                foreach (byte[] b in ByteUtil.UnwrapSegmentedBytes(state.buffer))
                {
                    /*try
                    {*/
                        ProcessBytes(b, sender, state.isAcknowledge);
                    /*}
                    catch
                    {
                        DisconnectClient(sender, DisconnectReason.ExceptionThrown);
                        return;
                    }*/
                }
                   
                Array.Clear(state.buffer, 0, state.buffer.Length);
            }
            else
            {
                Logger.Log($"Client { sender.ToIdentifiedString() } overloaded their receive thread.", LoggedImportance.CriticalWarning);

                DisconnectClient(sender, DisconnectReason.Overloaded);
                return;
            }

            state.isAcknowledge = false;

            if (!sender.Socket.Connected)
                return;

            sender.Socket.BeginReceive(state.buffer, 0, SendReceiveBufferSizePerClient, 0, ReadCallback, state);
        }

        public bool ProcessBytes(byte[] buffer, BaseClientData<TIdentity> sender, bool isAcknowledge = false)
        {
            if (sender == null || buffer == null || buffer.Length <= 0)
                return false;

            ByteWrapper bw = ByteWrapper.UnWrap(buffer);

            if (!bw.Integrity)
            {
                Logger.Log($"Integrity of packet send by { sender.ToIdentifiedString() } was not valid.", LoggedImportance.CriticalWarning);

                DisconnectClient(sender, DisconnectReason.FaultyIntegrity);
                return false;
            }

            if (isAcknowledge)
            {
                if (bw.ContentType != BytesContentType.AuthorizationScheme)
                {
                    DisconnectClient(sender, DisconnectReason.Unauthorized);
                    return false;
                }

                Stack<byte[]> authInfo = ByteUtil.ToSegmentStack(bw.DataBuffer, 0, bw.DataBuffer.Length);
                if (authInfo.Count == 6 || authInfo.Count == 7)
                {
                    const long State = 9896;

                    XorShift.Initiate(State);

                    string clientStxVersion = Encoding.ASCII.GetString(ByteUtil.Xor(authInfo.Pop(), XorShift.NextByte()));
                    string clientAppName = Encoding.ASCII.GetString(ByteUtil.Xor(authInfo.Pop(), XorShift.NextByte()));
                    string clientAppVersion = Encoding.ASCII.GetString(ByteUtil.Xor(authInfo.Pop(), XorShift.NextByte()));
                    string clientAppKey = Encoding.ASCII.GetString(ByteUtil.Xor(authInfo.Pop(), XorShift.NextByte()));
                    string clientID = Encoding.ASCII.GetString(ByteUtil.Xor(authInfo.Pop(), XorShift.NextByte()));
                    string clientAuthToken = Encoding.ASCII.GetString(ByteUtil.Xor(authInfo.Pop(), XorShift.NextByte()));
                    string clientName = null;
                    if (authInfo.Count > 0)
                        clientName = Encoding.ASCII.GetString(ByteUtil.Xor(authInfo.Pop(), XorShift.NextByte()));

                    if (clientStxVersion != StxServer.ServerStxVersion
                       || clientAppVersion != ApplicationVersion
                       || clientAppName != ApplicationName
                       || clientAppKey != ApplicationKey)
                    {
                        byte[] updateLinkBytes = Encoding.ASCII.GetBytes(UpToDateApplicationUrl);
                        Send(sender.Socket, updateLinkBytes, BytesContentType.UpdateLocation);

                        DisconnectClient(sender, DisconnectReason.UpdateRequired);
                        return false;
                    }

                    if (ConnectedClients.ContainsKey(clientID))
                    {
                        Logger.Log("A client cannot connect twice: " + sender.ToIdentifiedString(), LoggedImportance.CriticalWarning);

                        DisconnectClient(sender, DisconnectReason.Unauthorized);
                        return false;
                    }

                    if (!StringChecker.IsValidID(clientID) || clientID == NetworkIdentity.UnknownID)
                    {
                        Logger.Log("A client cannot have this weird ID: " + sender.ToIdentifiedString(), LoggedImportance.CriticalWarning);

                        DisconnectClient(sender, DisconnectReason.Unauthorized);
                        return false;
                    }

                    if (BannedClients.Contains(clientID))
                    {
                        Logger.Log("A banned client cannot connect: " + sender.ToIdentifiedString(), LoggedImportance.CriticalWarning);

                        DisconnectClient(sender, DisconnectReason.Unauthorized);
                        return false;
                    }

                    sender.Load(clientID);

                    if (!sender.Authorize(clientAuthToken))
                    {
                        DisconnectClient(sender, DisconnectReason.Unauthorized);
                        return false;
                    }

                    UnknownClients.Remove(sender);

                    ConnectedClients.TryAdd(sender.NetworkID, sender);
                    ClientRegisterer<TIdentity>.RegisteredClients.Add(sender.Identity);

                    if (sender.FirstTimeConnect)
                    {
                        // New client.
                        Logger.Log($"A very new client has been registered: " + sender.ToIdentifiedString(), LoggedImportance.Debug);

                        ServerStats.registeredClients++;
                    }

                    if (!string.IsNullOrEmpty(clientName) && StringChecker.IsValidName(clientName))
                    {
                        Logger.Log("Client name was directly send: " + clientName, LoggedImportance.Debug);

                        sender.Name = clientName;
                    }

                    sender.Identity.CameOnline();
                    sender.Save();

                    Packet p = GetAcknowledgmentPacket(sender, sender.FirstTimeConnect);
                    Send(sender.Socket, p.ToBytes());

                    Logger.Log("Welcome " + sender.ToIdentifiedString(), LoggedImportance.Successful);
                }
                else
                {
                    Logger.Log("Not enough auth info.", LoggedImportance.CriticalWarning);

                    DisconnectClient(sender, DisconnectReason.Unauthorized);
                    return false;
                }
            }
            else
            {
                if (bw.DataBuffer.Length > 0 && bw.ContentType != null)
                {
                    switch(bw.ContentType)
                    {
                        case BytesContentType.Ping:
                            ServerStats.pings++;
                            sender.LastPingReceived = DateTime.UtcNow;
                            return true;

                        case BytesContentType.Packet:
                            ServerStats.packets++;

                            Packet received = null;//Packet.AnyPacketFromBytes(buffer);

                            try
                            {
                                received = Bytifier.ObjectSize<Packet>(bw.DataBuffer, 0, bw.DataBuffer.Length);
                            }
                            catch (Exception e)
                            {
                                Logger.LogException(e, sender.ToIdentifiedString());
                                return false;
                            }

                            if (received == null)
                                return false;

                            // This is not possible, and will exploit god mode.
                            if (received.SenderID == NetworkID || received.SenderID != sender.NetworkID)
                            {
                                DisconnectClient(sender, DisconnectReason.FalseIdentity);

                                return false;
                            }

                            else if (received is RequestPacket)
                                return ServerRequest((RequestPacket)received, sender);

                            return true;

                        default:
                            Logger.Log($"Content type send by { sender.ToIdentifiedString() } is not supported: { bw.ContentType }", LoggedImportance.CriticalWarning);
                            return false;
                    }
                }
            }

            return true;
        }

        public virtual BaseClientData<TIdentity> GetClientDataForAccepted(Socket accepted)
        {
            var cd = new BaseClientData<TIdentity>(accepted);

            cd.MaxReceivesPerToken = MaxReceivesPerSecondPerClient;
            cd.MaxTimeoutSeconds = MaxTimeoutSeconds;
            cd.PingIntervalSeconds = PingIntervalSeconds;
            cd.TimedOutAction = new Action<BaseClientData<TIdentity>>((c) =>
            {
                DisconnectClient(c, DisconnectReason.TimedOut);
            });
            cd.Connected = true;

            return cd;
        }

        public void Announce(string message)
        {
            foreach (var d in GetConnectedClients())
                Send(d, Encoding.ASCII.GetBytes(message), BytesContentType.Announcement);
        }

        public static void Send(IBaseClientData to, byte[] buffer, BytesContentType contentType = BytesContentType.Packet)
            => Send(to?.Socket, buffer, contentType);

        public static void Send(Socket to, byte[] buffer, BytesContentType contentType = BytesContentType.Packet)
        {
            if (to == null || buffer == null || buffer.Length <= 0 || !to.Connected)
                return;

            buffer = ByteWrapper.Wrap(buffer, contentType);
            buffer = ByteUtil.WrapSegmentedBytes(buffer);

            ServerStats.bytesSend += buffer.LongLength;

            try
            {
                to.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallback, to);
            }
            catch
            {}
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;

                if (handler.Connected)
                    handler.EndSend(ar);
            }
            catch
            {}
        }

        public virtual Packet GetAcknowledgmentPacket(BaseClientData<TIdentity> forClient, bool isFirstTimeConnecting)
        {
            Packet p = new Packet(NetworkID);
            p.Data.Add("_FirstTime", isFirstTimeConnecting);
            p.Data.Add("You", forClient.Identity);
            return p;
        }

        public Packet GetNewPacket()
        {
            return new Packet(NetworkID);
        }

        public RequestPacket GetNewRequestPacket(string requestItemName, RequestPacket.RequestResponseDelegate requestResponse)
        {
            return new RequestPacket(NetworkID, requestItemName, requestResponse);
        }

        public bool ServerRequest(RequestPacket packet, BaseClientData<TIdentity> sender)
        {
            ServerStats.requests++;

            bool result = true;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (packet.DidRespond())
            {
                //Our request got responded => invoke method associated with request.
                packet.InvokeResponseMethods();
            }
            else
            {
                if (InvokeRequestHandlers(packet, sender))
                {
                    // We found a handler and it was interpret.

                    if (!packet.DidRespond())
                    {
                        packet.ResponseFail();

                        result = false;
                    }
                }
                else
                {
                    // No handler was found for the request.

                    packet.ResponseUnknown();
                }

                packet.SetResponderID(NetworkID);

                if (packet.ShouldRespond)
                    Send(sender.Socket, packet.ToBytes());

                if (packet.ResponseObject as string == RequestPacket.RequestResponseFailed)
                {
                    ServerStats.failedRequests++;

                    Logger.Log("Response to packet was marked as failed; Requested item: " + packet.RequestItemName, LoggedImportance.Warning);
                }
                else if (packet.ResponseObject as string == RequestPacket.RequestResponseUnknown)
                {
                    ServerStats.unknownRequests++;

                    Logger.Log("Response to packet was marked as unknown request; Requested item: " + packet.RequestItemName, LoggedImportance.CriticalWarning);
                }
                else
                {
                    ServerStats.okeyRequests++;
                }
            }

            sw.Stop();
            ServerStats.timeSpendOnRequests += sw.ElapsedMilliseconds;

            //Console.Title = sw.ElapsedMilliseconds.ToString();

            return result;
        }

        /// <summary>
        /// Invokes handlers for specific requests on this server.
        /// </summary>
        /// <param name="request">The request packet that needs a handler.</param>
        /// <param name="sender">The sender of this packet.</param>
        /// <returns>The fact that it found and invoked a handler.</returns>
        public virtual bool InvokeRequestHandlers(RequestPacket request, BaseClientData<TIdentity> sender)
        {
            if (RequestHandlers.ContainsKey(request.RequestItemName))
            {
                RequestHandlers[request.RequestItemName].Invoke(request, sender);

                return true;
            }

            return false;
        }

        #region Default Request Types

        private void RequestPing(RequestPacket packet, BaseClientData<TIdentity> sender)
        {
            packet.ResponseOk();
        }

        private void RequestDisconnect(RequestPacket packet, BaseClientData<TIdentity> sender)
        {
            DisconnectClient(sender, DisconnectReason.RequestedOfflineIntended);

            packet.DoNotRespond();
        }

        private void RequestSetName(RequestPacket packet, BaseClientData<TIdentity> sender)
        {
            if (!packet.Data.Requires<string>("NewName"))
                return;

            string newName = ((string)packet.Data["NewName"]).Trim();

            if (!ClientRegisterer<TIdentity>.IsNamePicked(newName) && StringChecker.IsValidName(newName))
            {
                sender.Name = newName;
                sender.Save();

                packet.ResponseOk();
            }
        }

        private void RequestTest(RequestPacket packet, BaseClientData<TIdentity> sender)
        {
            if (!packet.Data.Requires<string>("Missing"))
            {
                packet.ResponseRequires<string>("Missing");

                return;
            }

            string value = packet.Data["Missing"].ToString();

            Logger.Info("Value was send: " + value);

            packet.Respond(696396);
        }

        #endregion

        public bool DisconnectSocket(Socket client, DisconnectReason reason)
        {
            try
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                return true;
            }
            catch (Exception e)
            {
                Logger.LogException(e, "Could not disconnect client");
                return false;
            }
            finally
            {
                ServerStats.IncreaseFor(reason);

                Logger.Log($"Client disconnected ({ reason }): " + client.RemoteEndPoint);
            }
        }

        public bool DisconnectClient(BaseClientData<TIdentity> client, DisconnectReason reason)
        {
            if (client == null || !client.Connected)
                return false;

            client.DisconnectMe(reason);

            BaseClientData<TIdentity> tr;
            ConnectedClients.TryRemove(client.NetworkID, out tr);
            UnknownClients.Remove(client);

            ServerStats.IncreaseFor(reason);

            Logger.Log($"Client disconnected ({ reason }): " + client.ToIdentifiedString());
            return true;
        }
    }
}
