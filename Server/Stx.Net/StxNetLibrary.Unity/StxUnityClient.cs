using Stx.Logging;
using Stx.Net.RoomBased;
using Stx.Net.Unity.UI;
using Stx.Serialization;
using Stx.Utilities;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Stx.Net.Unity
{
    public class StxUnityClient : MonoBehaviour
    {
        [System.Serializable]
        public class ApplicationInfo
        {
            [Header("Key is required")]
            public string applicationKey;
            public string applicationName = StxNet.DefaultApplicationName;
            public string applicationVersion = StxNet.DefaultApplicationVersion;
        }

        [Header("Stx.Net v" + StxNet.ClientStxVersion)]
        [Space]
        [Header("Server connection")]
        public string host = "127.0.0.1:1987";
        public string clientIDOverride = null;
        public bool connectOnStart = true;
        public bool connectAsync = true;
        public ApplicationInfo applicationInfo;
        [Range(0f, 20f)]
        public float doConnectDelay = 1f;
        [Range(2.5f, 45f)]
        public float reconnectTime = 5f;
        [Header("Environment")]
        public string mainScene = "GameMenu";
        public string lobbyScene = "Lobby";
        public string gameScene = "Game";
        [Tooltip("Implement your own scene switching methods.")]
        public UnityStringEvent onDoLoadScene;
        [Header("Events")]
        public UnityVoidEvent onConnecting;
        public UnityBoolEvent onConnected;
        public UnityVoidEvent onDisconnected;
        public UnityStringEvent onUpdateRequired;

        /// <summary>
        /// The actual client object, modify its settings here.
        /// </summary>
        public static Client C { get; private set; }
        /// <summary>
        /// A collection of functions that will send requests to the server.
        /// </summary>
        public static ServerFunctions F { get; private set; }
        /// <summary>
        /// Your client identity object; gets downloaded when the connection is established.
        /// </summary>
        public static ClientIdentity You
        {
            get
            {
                return C.You;
            }
        }
        public delegate void InputRequiredDelegate(string mainInstruction, string subInstruction, Func<string, bool> submitResult); // String is null if canceled, return to validate.
        public InputRequiredDelegate InputRequired;
        public delegate void AlertDelegate(string message, string title, bool isPositive);
        public AlertDelegate Alert;

        public static Logging.ILogger Logger { get; set; } = StxNet.DefaultLogger;
        public static StxUnityClient Instance { get; private set; } = null;

        public const int DEFAULT_PORT = 1987;

        void Awake()
        {
            StxNet.DefaultLogger = new UnityLogger();

            if (Instance == null)
            {
                name = "[StxClient]";

                Instance = this;
            }
            else
            {
                Destroy(gameObject);

                return;
            }
        }

        void Start()
        {
            // Let the system know we are working with multiple threads, 
            // the Unity thread and the receive/send thread.
            ThreadSafeData.MultiThreadOverride = true;
            Bytifier.Logger = StxNet.DefaultLogger;
            Logger = StxNet.DefaultLogger;

            onConnecting?.Invoke();

            Logger.Log($"Connecting to { host }...");

            ConnectionStartInfo s = new ConnectionStartInfo(host, applicationInfo.applicationKey, true);
            if (!string.IsNullOrEmpty(clientIDOverride))
                s.ClientID = clientIDOverride;
            s.ApplicationName = applicationInfo.applicationName;
            s.ApplicationVersion = applicationInfo.applicationVersion;
            s.ConnectOnConstruct = false;
            s.ConnectAsync = connectAsync;

            C = new Client(s);

            StartCoroutine(IConnect(host));
        }

        private IEnumerator IConnect(string host)
        {
            yield return new WaitForSeconds(doConnectDelay);

            C.OnCannotConnect += Client_OnCannotConnect;
            C.OnConnected += Client_Connected;
            C.OnDisconnected += Client_Disconnected;
            C.OnUpdateRequired += Client_OnUpdateRequired;
            C.OnKickedFromRoom += Client_OnKickedFromRoom;
            C.OnGameStateChange += Client_OnGameStateChange;

            if (connectOnStart)
                C.Connect();

            Logger.Log($"Your <color=#00FF00>client ID</color>: <b><color=#00AA00>{ C.NetworkID }</color></b>");
            Logger.Log($"Your <color=#00FFFF>auth token</color>: <b><color=#00AAAA>{ C.ClientAuthorisationToken }</color></b>");

            F = new ServerFunctions(C);
        }

        public void AskForInput(string mainInstruction, string subInstruction, Func<string, bool> submitResult)
        {
            if (InputRequired == null)
            {
                Logger.Warning("There was asked for user input but the input method was not defined (See StxUnityClient.OnInputRequired). The request was marked as canceled.");
            }
            else
            {
                InputRequired.Invoke(mainInstruction, subInstruction, submitResult);
            }
        }

        public void DisplayAlert(string message, string title = "Alert", bool isPositive = true)
        {
            if (Alert == null)
            {
                Logger.Warning("There was an alert requested but the alert method was not defined (See StxUnityClient.OnAlert). The request was marked as canceled.");
            }
            else
            {
                Alert.Invoke(message, title, isPositive);
            }
        }

        private void Client_OnCannotConnect(System.Exception ex)
        {
            Logger.Log($"Cannot connect, trying again in { reconnectTime } seconds...", Logging.LoggedImportance.Warning);

            Invoke(nameof(Reconnect), reconnectTime);
        }

        private void Reconnect()
        {
            C.Connect();
        }

        private void Client_OnGameStateChange(GameState newGameState)
        {
            Logger.Log($"<i>Game state has changed to { newGameState.ToString() }</i>", Logging.LoggedImportance.Debug);

            if (newGameState == GameState.InGame)
                SceneSwitchGame();
            else if (newGameState == GameState.InLobby)
                SceneSwitchLobby();
        }

        private void Client_OnKickedFromRoom()
        {
            Logger.Log("<b>The client got kicked from the room.</b>", Logging.LoggedImportance.Warning);

            SceneSwitchMain();
        }

        private void Client_OnUpdateRequired(string updateDownloadLocation)
        {
            Logger.Log("<b><color=#FF5555>Software update required!</color></b>");

            //Alert("A software update is required for playing!", "Alert");

            onUpdateRequired?.Invoke(updateDownloadLocation);
        }

        private void Client_Disconnected(DisconnectReason? reason)
        {
            Logger.Log("<b><color=#FF5555>The client got disconnected!</color></b> Reason: " + reason);

            onDisconnected?.Invoke();
        }

        void Update()
        {
            // Invoking events/data handlers thread-safely...
            ThreadSafeData.CheckForAllData();
        }

        void OnApplicationQuit()
        {
            Logger.Log("<b><color=#FFFF33>Application stopped, disconnecting...</color></b>");

            //client.RequestDisconnect();
            C.StopConnection();
        }

        private void Client_Connected(bool firstTime)
        {
            Logger.Log("<b><color=#33FF33>Connection established.</color></b>", Logging.LoggedImportance.Successful);

            onConnected?.Invoke(firstTime);

            //Alert($"Welcome back { C.You.ShortDisplayName }!", "Have a wonderful time scaring around!");
        }

        /*public static void Log(string str)
        {
            Debug.Log(DEBUG_PREFIX + str);
        }

        public static void Warning(string str)
        {
            Debug.LogWarning(DEBUG_WARNING_PREFIX + str);
        }

        public static void Error(string str)
        {
            Debug.LogError(DEBUG_ERROR_PREFIX + str);
        }*/

        public void SceneSwitchLobby()
        {
            SceneSwitch(lobbyScene);
        }

        public void SceneSwitchMain()
        {
            SceneSwitch(mainScene);
        }

        public void SceneSwitchGame()
        {
            SceneSwitch(gameScene);
        }

        public void SceneSwitch(string sceneName)
        {
            if (onDoLoadScene == null)
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                onDoLoadScene.Invoke(sceneName);
            }
        }
        /*private void Client_Received(Packet p)
        {
            incoming.Enqueue(p);
        }*/
    }


    /*public class StxUnityClient : MonoBehaviour
    {
        public string host;
        public string clientID;

        public Socket Socket { get; private set; }
        public string ClientID { get; private set; }
        public IPAddress ConnectAddress { get; private set; }
        public int Port { get; private set; }
        public DataReceiver DataReceiver { get; set; }
        public string ClientAuthorisationToken { get; private set; } = "5b1160ca-9d43-4fd0-aa99-61cb8f62d0d9";

        public delegate void ReceiveDelegate(Packet p);
        public event ReceiveDelegate Received;

        public delegate void ConnectedDelegate(bool firstTime);
        public event ConnectedDelegate Connected;

        private const string DEBUG_PREFIX = "[StxNet] ";
        private const string DEBUG_ERROR_PREFIX = "[StxNet/Error] ";

        public const int DEFAULT_PORT = 1987;

        void Start()
        {
            IPAddress ip = IPAddress.None;
            int port = DEFAULT_PORT;
            if (!IPUtil.ParseIPAndPort(host, out ip, out port, DEFAULT_PORT))
            {
                Debug.LogError(DEBUG_ERROR_PREFIX + "Could not parse IP and port from host input.");
                return;
            }

            ErrorHandler.OnError += (e, c) =>
            {
                Debug.LogError(DEBUG_ERROR_PREFIX + e.Message + $" (Error code { c })");
            };

            this.ConnectAddress = ip;
            this.Port = port;

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint serverEndpoint = new IPEndPoint(ip, port);

            if (clientID == null)
                clientID = Guid.NewGuid().ToString();

            this.ClientID = clientID;
            this.DataReceiver = new DataReceiver();
            this.DataReceiver.AddHandler(new DataHandler<bool>("FirstTime", (b) =>
            {
                Connected?.Invoke(b);
            }));

            StartCoroutine(IConnect(serverEndpoint));
        }

        

        //private Thread receiveThread;




        public Packet GetNewPacket()
        {
            return new Packet(ClientID);
        }

        public RequestPacket GetNewRequestPacket(string requestItemName, RequestPacket.RequestResponseDelegate requestResponse)
        {
            return new RequestPacket(ClientID, requestItemName, requestResponse);
        }

        public void SendToServer(byte[] bytes)
        {
            Socket.TrySend(bytes);
        }

        public void StopConnection()
        {
            try
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();

                StopAllCoroutines();
            }
            catch (Exception e)
            {
                ErrorHandler.Error(e, 4);
                StopConnection();
            }
        }

        private IEnumerator IConnect(IPEndPoint serverEndpoint)
        {
            yield return new WaitForSeconds(0.25f);

            try
            {
                Socket.Connect(serverEndpoint);

                //receiveThread = new Thread(DataReceive);
                //receiveThread.Start();
                StartCoroutine(IDataReceive());

                //Send acknowledge so the server knows our ClientID.
                Packet acknowledge = GetNewPacket();
                Socket.TrySend(acknowledge.ToBytes());

                //Authorize ourself on the server.
                RequestPacket auth = GetNewRequestPacket("Authorize", (obj) =>
                {
                    if (obj.ToString() == RequestPacket.FAIL_RESPONSE)
                        ErrorHandler.Error("Could not authorize on server.", 6);
                });
                auth.Data.Add("Token", ClientAuthorisationToken);
                Socket.TrySend(auth.ToBytes());
            }
            catch (Exception e)
            {
                ErrorHandler.Error(e, 0);
            }

            
        }

        private IEnumerator IDataReceive()
        {
            byte[] buffer;
            int sizeReceived = 0;

            while (true)
            {
                buffer = new byte[Socket.SendBufferSize];
                try
                {
                    sizeReceived = Socket.Receive(buffer);
                }
                catch (Exception e)
                {
                    ErrorHandler.Error(e, 7);
                }

                if (sizeReceived > 0)
                {
                    Packet p = Packet.AnyPacketFromBytes(buffer);

                    if (p is RequestPacket)
                    {
                        RequestPacket rp = (RequestPacket)p;

                        if (rp.DidRespond())
                        {
                            //Our request got responded => invoke method associated with request.
                            rp.InvokeResponseMethods();
                        }
                        else
                        {
                            //We have to respond to packet.
                            if (rp.GetRequestedItemName() == "Ping")
                            {
                                rp.ResponseOk();
                                rp.SetResponderID(ClientID);
                                Socket.TrySend(rp.ToBytes());
                            }
                        }
                    }
                    else if (p is Packet)
                    {
                        Received?.Invoke(p);

                        DataReceiver.Received(p);
                    }
                }

                yield return new WaitForSeconds(0f);
            }
        }

    }*/
}
