using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WebSocketSharp;

[Serializable]
public class FNAF1GameSettings
{
    public float secondsPerHour;
    public float secondsPerMove;
    public float maxMoveTimeRandomness;
    public float decreaseMoveTimeOverNight;
    public float startingMoveTime;
    public float initialPower;
    public int hours;
}

[Serializable]
public class FNAF1Game
{
    public FNAF1GameSettings settings;
    public int chicaLocation; // Which camera
    public int chicaLocationState; // Which image to display on camera
    public int freddyLocation;
    public int freddyLocationState;
    public int bonnieLocation;
    public int bonnieLocationState;
    public int foxyLocation;
    public int foxyLocationState;
    public float powerLeft;
    public int currentHour;
    public long startTimeStamp;
    public bool leftLight;
    public bool rightLight;
    public bool leftDoor;
    public bool rightDoor;
    public int selectedCameraNumber;
    public string attackingMonster;
    public bool guardAlive;
}

[Serializable]
public class FNAFUser
{
    public int id;
    public string name;

    public override string ToString()
    {
        return $"User id={id} name={name}";
    }
}

[Serializable]
public class FNAFRoomUser
{
    public FNAFUser user;
    public bool ready;
    public string role;

    public override string ToString()
    {
        return user.ToString() + " ready=" + (ready ? "yes" : "no") + " role=" + role;
    }
}

[Serializable]
public class FNAFRoom
{
    public string id;
    public string name;
    public int ownerId;
    public string ownerName;
    public int playerCount;
    public List<FNAFRoomUser> users;
    public int maxPlayers;
    public bool isPrivate;
    public bool inGame;
    public FNAF1Game game;

    public override string ToString()
    {
        return $"Room id={id} name={name} ownerName={ownerName} ownerId={ownerId} playerCount={playerCount}/{maxPlayers} inGame={(inGame ? "yes" : "no")} private={(isPrivate ? "yes" : "no")}";
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
public class FNAFStartRequest
{
    public bool ready;
}

[Serializable]
public class FNAFStartResponse
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
public class FNAF1OfficeChangeRequest
{
    public bool leftLight;
    public bool rightLight;
    public bool leftDoor;
    public bool rightDoor;
    public int selectedCameraNumber;
}

[Serializable]
public class FNAF1AttackRequest
{
    public string monster;
}

[Serializable]
public class FNAFConfig
{
    public int version;
    public long createdTimestamp;
    public string token;
}

public class FNAFClient : MonoBehaviour
{
    public string connectionUrl = "ws://localhost:8080";

    public event EventHandler OnConnected;
    public event EventHandler<CloseEventArgs> OnDisconnected;

    public event EventHandler<FNAFRoomChangeEvent> OnRoomChangeEvent;

    public event EventHandler<FNAFCreateRoomResponse> OnCreateRoomResponse;
    public event EventHandler<FNAFJoinRoomResponse> OnJoinRoomResponse;
    public event EventHandler<FNAFLeaveRoomResponse> OnLeaveRoomResponse;
    public event EventHandler<FNAFLoginResponse> OnLoginResponse;
    public event EventHandler<FNAFRegisterResponse> OnRegisterResponse;
    public event EventHandler<FNAFMatchmakingResponse> OnMatchmakingResponse;
    public event EventHandler<FNAFStartResponse> OnStartResponse;
    public event EventHandler<FNAF1MoveResponse> OnFNAF1MoveResponse;

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
        if (File.Exists(configPath))
        {
            string fileContents = File.ReadAllText(configPath);
            config = JsonUtility.FromJson<FNAFConfig>(fileContents);
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
        File.WriteAllText(configPath, JsonUtility.ToJson(config));
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
                var joinRoomData = JsonUtility.FromJson<FNAFJoinRoomResponse>(jsonText);
                if (joinRoomData.ok)
                    currentRoom = joinRoomData.room;
                OnJoinRoomResponse?.Invoke(null, joinRoomData);
                break;

            case nameof(FNAFLeaveRoomResponse):
                var leaveRoomData = JsonUtility.FromJson<FNAFLeaveRoomResponse>(jsonText);
                if (leaveRoomData.ok)
                    currentRoom = null;
                OnLeaveRoomResponse?.Invoke(null, leaveRoomData);
                break;

            case nameof(FNAFCreateRoomResponse):
                var newRoomData = JsonUtility.FromJson<FNAFCreateRoomResponse>(jsonText);
                if (newRoomData.ok)
                    currentRoom = newRoomData.room;
                OnCreateRoomResponse?.Invoke(null, newRoomData);
                break;

            case nameof(FNAFLoginResponse):
                OnLoginResponse?.Invoke(null, JsonUtility.FromJson<FNAFLoginResponse>(jsonText));
                break;

            case nameof(FNAFRegisterResponse):
                OnRegisterResponse?.Invoke(null, JsonUtility.FromJson<FNAFRegisterResponse>(jsonText));
                break;

            case nameof(FNAFMatchmakingResponse):
                OnMatchmakingResponse?.Invoke(null, JsonUtility.FromJson<FNAFMatchmakingResponse>(jsonText));
                break;

            case nameof(FNAFStartResponse):
                OnStartResponse?.Invoke(null, JsonUtility.FromJson<FNAFStartResponse>(jsonText));
                break;

            case nameof(FNAFRoomChangeEvent):
                var roomChangeEvent = JsonUtility.FromJson<FNAFRoomChangeEvent>(jsonText);
                currentRoom = roomChangeEvent.room;
                OnRoomChangeEvent?.Invoke(null, roomChangeEvent);
                break;

            case nameof(FNAF1MoveResponse):
                OnFNAF1MoveResponse?.Invoke(null, JsonUtility.FromJson<FNAF1MoveResponse>(jsonText));
                break;

            default:
                Debug.LogWarning("Received unknown message type from server: " + type);
                break;
        }
    }

    public void RegisterRequest(string name)
    {
        socket.Send(nameof(FNAFRegisterRequest) + ":" + JsonUtility.ToJson(new FNAFRegisterRequest() { name = name }));
    }

    public void LoginRequest(string token)
    {
        socket.Send(nameof(FNAFLoginRequest) + ":" + JsonUtility.ToJson(new FNAFLoginRequest() { token = token }));
    }

    public void CreateRoom(int maxPlayers, bool isPrivate)
    {
        socket.Send(nameof(FNAFCreateRoomRequest) + ":" + JsonUtility.ToJson(new FNAFCreateRoomRequest() { isPrivate = isPrivate, maxPlayers = maxPlayers }));
    } 

    public void JoinRoom(string id)
    {
        socket.Send(nameof(FNAFJoinRoomRequest) + ":" + JsonUtility.ToJson(new FNAFJoinRoomRequest() { id = id }));
    }

    public void LeaveRoom()
    {
        socket.Send(nameof(FNAFLeaveRoomRequest) + ":" + JsonUtility.ToJson(new FNAFLeaveRoomRequest()));
    }

    public void RequestMatchmaking()
    {
        socket.Send(nameof(FNAFMatchmakingRequest) + ":" + JsonUtility.ToJson(new FNAFMatchmakingRequest()));
    }

    public void StartGameRequest(bool ready)
    {
        socket.Send(nameof(FNAFStartRequest) + ":" + JsonUtility.ToJson(new FNAFStartRequest() { ready = ready }));
    }

    public void FNAF1RequestMove(string monster, int location, int locationState)
    {
        socket.Send(nameof(FNAF1MoveRequest) + ":" + JsonUtility.ToJson(new FNAF1MoveRequest() { monster = monster, location = location, locationState = locationState }));
    }

    public void FNAF1RequestOfficeChange(bool leftLight, bool leftDoor, bool rightLight, bool rightDoor, int selectedCameraNumber)
    {
        socket.Send(nameof(FNAF1OfficeChangeRequest) + ":" + JsonUtility.ToJson(new FNAF1OfficeChangeRequest() { leftLight = leftLight, leftDoor = leftDoor, rightLight = rightLight, rightDoor = rightDoor, selectedCameraNumber = selectedCameraNumber }));
    }

    public void FNAF1RequestAttack(string monster)
    {
        socket.Send(nameof(FNAF1AttackRequest) + ":" + JsonUtility.ToJson(new FNAF1AttackRequest() { monster = monster }));
    }
}