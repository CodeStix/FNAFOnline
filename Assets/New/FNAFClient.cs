using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;

[System.Serializable]
public class FNAF1OfficeCameraState
{
    public int cameraIndex;
    public FNAFUser byPlayer;
}

[System.Serializable]
public class FNAF1OfficeState
{
    public FNAF1OfficeState()
    {
        camerasLookedAt = new List<FNAF1OfficeCameraState>();
        powerLeft = 100f;
    }

    public int chicaLocation; // Which camera
    public int chicaLocationState; // Which image to display on camera
    public int freddyLocation;
    public int freddyLocationState;
    public int bonnieLocation;
    public int bonnieLocationState;
    public int foxyLocation;
    public int foxyLocationState;
    public bool leftLight;
    public bool rightLight;
    public bool leftDoor;
    public bool rightDoor;
    public float powerLeft;
    public List<FNAF1OfficeCameraState> camerasLookedAt;
}

[Serializable]
public class FNAFGame
{
    public string gameMode;
    public float secondsPerHour;
    public float secondsPerMove;
    public float maxMoveTimeRandomness;
    public float decreaseMoveTimeOverNight;
    public float startingMoveTime;
    public float initialPower;
    public int hours;
    public List<FNAFGamePlayer> players;
}

[Serializable]
public class FNAFGamePlayer
{
    public FNAFUser user;
    public bool alive;
    public FNAFUser controlledByUser;
    public long allowDistractTimestamp;
    public long allowMoveTimestamp;
}

//[Serializable]
//public class FNAF1Game
//{
//    public long startTimeStamp;
//    public int currentHour;
//    public FNAF1OfficeState office; // Used when gameMode is classic
//    public List<FNAFGamePlayer> players;
//}

[Serializable]
public class FNAFUser
{
    public int id;
    public string name;
    public int night;

    public override string ToString()
    {
        return $"User id={id} name={name} night={night}";
    }
}

[Serializable]
public class FNAFRoom
{
    public string id;
    public string name;
    public int ownerId;
    public string ownerName;
    public List<FNAFUser> users;
    public int playerCount;
    public int maxPlayers;
    public bool isPrivate;
    public bool inGame;
    public List<int> readyUserIds;
    public FNAFGame game;

    public override string ToString()
    {
        return $"Room id={id} name={name} ownerId={ownerId} playerCount={playerCount}/{maxPlayers} inGame={(inGame ? "yes" : "no")} private={(isPrivate ? "yes" : "no")} users=[{string.Join(",", users.Select((e) => e.ToString()))}]";
    }
}

[Serializable]
public class FNAFMatchmakingRequest
{
}

[Serializable]
public class FNAFMatchmakingResponse
{
    public FNAFRoom[] rooms;
}

[Serializable]
public class FNAFLoginRequest
{
    public string token;
}

[Serializable]
public class FNAFLoginResponse
{
    public FNAFUser user;
}

[Serializable]
public class FNAFRegisterRequest
{
    public string name;
}

[Serializable]
public class FNAFRegisterResponse
{
    public string token;
    public FNAFUser user;
}

[Serializable]
public class FNAFJoinRoomRequest
{
    public string id;
}

[Serializable]
public class FNAFJoinRoomResponse
{
    public bool ok;
    public FNAFRoom room;
}

[Serializable]
public class FNAFLeaveRoomRequest
{
}

[Serializable]
public class FNAFLeaveRoomResponse
{
    public bool ok;
}

[Serializable]
public class FNAFCreateRoomRequest
{
    public int maxPlayers;
    public bool isPrivate;
}

[Serializable]
public class FNAFCreateRoomResponse
{
    public bool ok;
    public FNAFRoom room;
}

[Serializable]
public class FNAFRoomChangeEvent
{
    public string eventType;
    public FNAFRoom room;
}

[Serializable]
public class FNAFReadyRequest
{
    public bool ready;
}

[Serializable]
public class FNAFReadyResponse
{
    public bool ok;
    public bool ready;
}

[Serializable]
public class FNAF1MoveRequest
{
    public string monster;
    public int location;
    public int locationState;
}

[Serializable]
public class FNAF1MoveResponse
{
    public bool ok;
    public bool gotCaught;
    public float cooldownTime;
}

[Serializable]
public class FNAF1AttackResponse
{
    public string monster;
    public bool ok;
}

[Serializable]
public class FNAF1DistractResponse
{
    public bool ok;
    public float cooldownTime;
}

[Serializable]
public class FNAF1OfficeChangeRequest
{
    public bool leftLight;
    public bool rightLight;
    public bool leftDoor;
    public bool rightDoor;
    public int selectedCameraNumber;
}

[Serializable]
public class FNAFChatRequest
{
    public string message;
}

[Serializable]
public class FNAFConfig
{
    public int version;
    public long createdTimestamp;
    public string token;
}

[Serializable]
public class FNAFUserChangeEvent
{
    public FNAFUser user;
}

[Serializable]
public class FNAFChatEvent
{
    public FNAFUser sender;
    public string message;
}

[Serializable]
public class FNAF1DistractRequest
{
    public string distraction;
}

[Serializable]
public class FNAF1AttackRequest
{
    public string monster;
}

[Serializable]
public class FNAF1DistractEvent
{
    public string distraction;
}

[Serializable]
public class FNAF1AttackEvent
{
    public string monster;
}

[Serializable]
public class FNAF1OfficeEvent
{
    public FNAF1OfficeState office;
    public float currentHour;
}

public class FNAFClient : MonoBehaviour
{
    // wss://fnafws.codestix.nl
    public string connectionUrl = "ws://localhost:8080";
    public bool testMode = false;

    public event EventHandler OnConnected;
    public event EventHandler<CloseEventArgs> OnDisconnected;

    public event EventHandler<FNAFRoomChangeEvent> OnRoomChangeEvent;
    public event EventHandler<FNAFUserChangeEvent> OnUserChangeEvent;
    public event EventHandler<FNAFChatEvent> OnChatEvent;
    public event EventHandler<FNAF1DistractEvent> OnFNAF1DistractEvent;
    public event EventHandler<FNAF1AttackEvent> OnFNAF1AttackEvent;
    public event EventHandler<FNAF1OfficeEvent> OnFNAF1OfficeEvent;

    public event EventHandler<FNAFCreateRoomResponse> OnCreateRoomResponse;
    public event EventHandler<FNAFJoinRoomResponse> OnJoinRoomResponse;
    public event EventHandler<FNAFLeaveRoomResponse> OnLeaveRoomResponse;
    public event EventHandler<FNAFLoginResponse> OnLoginResponse;
    public event EventHandler<FNAFRegisterResponse> OnRegisterResponse;
    public event EventHandler<FNAFMatchmakingResponse> OnMatchmakingResponse;
    public event EventHandler<FNAFReadyResponse> OnReadyResponse;
    public event EventHandler<FNAF1MoveResponse> OnFNAF1MoveResponse;
    public event EventHandler<FNAF1AttackResponse> OnFNAF1AttackResponse;
    public event EventHandler<FNAF1DistractResponse> OnFNAF1DistractResponse;

    private WebSocket socket;
    private FNAFConfig config;
    private ConcurrentQueue<string> incomingMessages = new ConcurrentQueue<string>();

    private FNAFUser me;
    private FNAFRoom currentRoom;

    //string oldConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"CodeStix/Net/local.json");
    private static string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.isEditor ? @"CodeStix/user-editor.json" : @"CodeStix/user.json");

    public static FNAFClient Instance { get; private set; }

    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("An instance of FNAFClient is already running");
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (incomingMessages.Count > 0)
        {
            if (incomingMessages.TryDequeue(out string message))
            {
                try
                {
                    HandleMessage(message);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error during HandleMessage: " + ex);
                }
            }
        }
    }

    public FNAFUser GetUser()
    {
        return me;
    }

    public FNAFRoom GetRoom()
    {
        return currentRoom;
    }

    private void LoadConfig()
    {
        if (File.Exists(configPath) && !(testMode && !Application.isEditor))
        {
            string fileContents = File.ReadAllText(configPath);
            config = JsonConvert.DeserializeObject<FNAFConfig>(fileContents);
        }
        else
        {
            config = new FNAFConfig() {
                version = 1,
                createdTimestamp = new DateTimeOffset().ToUnixTimeMilliseconds(), 
                token = null
            };
            SaveConfig();
        }
    }

    private void SaveConfig()
    {
        File.WriteAllText(configPath, JsonConvert.SerializeObject(config));
    }

    public void Connect()
    {
        LoadConfig();

        OnConnected += (_a, _b) =>
        {
            Debug.Log("[FNAFClient] Connected to WebSocket");

            if (config.token == null)
            {
                OnRegisterResponse += FNAFClient_OnRegisterResponse;
                RegisterRequest("Freddy");
            }
            else
            {
                OnLoginResponse += FNAFClient_OnLoginResponse;
                LoginRequest(config.token);
            }
        };
        OnDisconnected += (_a, _b) =>
        {
            Debug.LogWarning("[FNAFClient] Disconnected from WebSocket");
        };

        socket = new WebSocket(connectionUrl);
        socket.OnOpen += (sender, e) => incomingMessages.Enqueue("Connected:{}");
        socket.OnClose += (sender, e) => incomingMessages.Enqueue("Disconnected:{}");
        socket.OnMessage += Socket_OnMessage;
        socket.OnError += (_, e) =>
        {
            Debug.LogError("[FNAFClient] Error: " + e.Message);
        };

        socket.ConnectAsync();
    }

    private void FNAFClient_OnRegisterResponse(object sender, FNAFRegisterResponse e)
    {
        OnRegisterResponse -= FNAFClient_OnRegisterResponse;

        Debug.Log("Registered new user: " + e.token);

        me = e.user;
        config.token = e.token;
        SaveConfig();
    }

    private void FNAFClient_OnLoginResponse(object sender, FNAFLoginResponse e)
    {
        OnLoginResponse -= FNAFClient_OnLoginResponse;

        Debug.Log("Logged in: " + e.user);

        me = e.user;
    }

    private void Socket_OnMessage(object sender, MessageEventArgs e)
    {
        if (!e.IsText || e.IsPing) return;

        incomingMessages.Enqueue(e.Data);
    }

    private void HandleMessage(string rawData)
    {
        int splitIndex = rawData.IndexOf(':');

        if (splitIndex < 0)
            throw new Exception("Invalid data");

        string type = rawData.Substring(0, splitIndex);
        string jsonText = rawData.Substring(splitIndex + 1);

        Debug.Log("Received " + rawData);

        switch (type)
        {
            case "Connected":
                OnConnected?.Invoke(null, null);
                break;

            case "Disconnected":
                OnDisconnected?.Invoke(null, null);
                break;

            case nameof(FNAFJoinRoomResponse):
                var joinRoomData = JsonConvert.DeserializeObject<FNAFJoinRoomResponse>(jsonText);
                if (joinRoomData.ok)
                    currentRoom = joinRoomData.room;
                OnJoinRoomResponse?.Invoke(null, joinRoomData);
                break;

            case nameof(FNAFLeaveRoomResponse):
                var leaveRoomData = JsonConvert.DeserializeObject<FNAFLeaveRoomResponse>(jsonText);
                if (leaveRoomData.ok)
                    currentRoom = null;
                OnLeaveRoomResponse?.Invoke(null, leaveRoomData);
                break;

            case nameof(FNAFCreateRoomResponse):
                var newRoomData = JsonConvert.DeserializeObject<FNAFCreateRoomResponse>(jsonText);
                if (newRoomData.ok)
                    currentRoom = newRoomData.room;
                OnCreateRoomResponse?.Invoke(null, newRoomData);
                break;

            case nameof(FNAFLoginResponse):
                OnLoginResponse?.Invoke(null, JsonConvert.DeserializeObject<FNAFLoginResponse>(jsonText));
                break;

            case nameof(FNAFRegisterResponse):
                OnRegisterResponse?.Invoke(null, JsonConvert.DeserializeObject<FNAFRegisterResponse>(jsonText));
                break;

            case nameof(FNAFMatchmakingResponse):
                OnMatchmakingResponse?.Invoke(null, JsonConvert.DeserializeObject<FNAFMatchmakingResponse>(jsonText));
                break;

            case nameof(FNAFReadyResponse):
                OnReadyResponse?.Invoke(null, JsonConvert.DeserializeObject<FNAFReadyResponse>(jsonText));
                break;

            case nameof(FNAFRoomChangeEvent):
                var roomChangeEvent = JsonConvert.DeserializeObject<FNAFRoomChangeEvent>(jsonText);
                currentRoom = roomChangeEvent.room;
                OnRoomChangeEvent?.Invoke(null, roomChangeEvent);
                break;

            case nameof(FNAFUserChangeEvent):
                var userChangeEvent = JsonConvert.DeserializeObject<FNAFUserChangeEvent>(jsonText);
                me = userChangeEvent.user;
                OnUserChangeEvent?.Invoke(null, userChangeEvent);
                break;

            case nameof(FNAF1MoveResponse):
                OnFNAF1MoveResponse?.Invoke(null, JsonConvert.DeserializeObject<FNAF1MoveResponse>(jsonText));
                break;

            case nameof(FNAF1AttackEvent):
                OnFNAF1AttackEvent?.Invoke(null, JsonConvert.DeserializeObject<FNAF1AttackEvent>(jsonText));
                break;

            case nameof(FNAF1DistractEvent):
                OnFNAF1DistractEvent?.Invoke(null, JsonConvert.DeserializeObject<FNAF1DistractEvent>(jsonText));
                break;

            case nameof(FNAF1AttackResponse):
                OnFNAF1AttackResponse?.Invoke(null, JsonConvert.DeserializeObject<FNAF1AttackResponse>(jsonText));
                break;

            case nameof(FNAF1DistractResponse):
                OnFNAF1DistractResponse?.Invoke(null, JsonConvert.DeserializeObject<FNAF1DistractResponse>(jsonText));
                break;

            case nameof(FNAFChatEvent):
                OnChatEvent?.Invoke(null, JsonConvert.DeserializeObject<FNAFChatEvent>(jsonText));
                break;

            case nameof(FNAF1OfficeEvent):
                OnFNAF1OfficeEvent?.Invoke(null, JsonConvert.DeserializeObject<FNAF1OfficeEvent>(jsonText));
                break;

            default:
                Debug.LogWarning("Received unknown message type from server: " + type);
                break;
        }
    }

    public void Send(string messageType, object value)
    {
        socket.Send(messageType + ":" + JsonConvert.SerializeObject(value));
    }

    public void RegisterRequest(string name)
    {
        Send(nameof(FNAFRegisterRequest), new FNAFRegisterRequest() { name = name });
    }

    public void LoginRequest(string token)
    {
        Send(nameof(FNAFLoginRequest), new FNAFLoginRequest() { token = token });
    }

    public void CreateRoom(int maxPlayers, bool isPrivate)
    {
        Send(nameof(FNAFCreateRoomRequest), new FNAFCreateRoomRequest() { isPrivate = isPrivate, maxPlayers = maxPlayers });
    } 

    public void JoinRoom(string id)
    {
        Send(nameof(FNAFJoinRoomRequest), new FNAFJoinRoomRequest() { id = id });
    }

    public void LeaveRoom()
    {
        Send(nameof(FNAFLeaveRoomRequest), new FNAFLeaveRoomRequest());
    }

    public void RequestMatchmaking()
    {
        Send(nameof(FNAFMatchmakingRequest), new FNAFMatchmakingRequest());
    }

    public void ReadyRequest(bool ready)
    {
        Send(nameof(FNAFReadyRequest), new FNAFReadyRequest() { ready = ready });
    }

    public void FNAF1RequestMove(string monster, int location, int locationState)
    {
        Send(nameof(FNAF1MoveRequest), new FNAF1MoveRequest() { monster = monster, location = location, locationState = locationState });
    }

    public void FNAF1RequestOfficeChange(bool leftLight, bool leftDoor, bool rightLight, bool rightDoor, int selectedCameraNumber)
    {
        Send(nameof(FNAF1OfficeChangeRequest), new FNAF1OfficeChangeRequest() { leftLight = leftLight, leftDoor = leftDoor, rightLight = rightLight, rightDoor = rightDoor, selectedCameraNumber = selectedCameraNumber });
    }

    public void FNAF1RequestAttack(string monster)
    {
        Send(nameof(FNAF1AttackRequest), new FNAF1AttackRequest() { monster = monster });
    }

    public void FNAF1RequestDistract(string distraction)
    {
        Send(nameof(FNAF1DistractRequest), new FNAF1DistractRequest() { distraction = distraction });
    }

    public void ChatRequest(string message)
    {
        Send(nameof(FNAFChatRequest), new FNAFChatRequest() { message = message });
    }
}