using System;
using System.Collections.Concurrent;
using System.IO;
using UnityEngine;
using WebSocketSharp;

[Serializable]
public class FNAFRoom
{
    public string id;
    public string name;
    public string ownerName;
    public int playerCount;
    public int maxPlayers;
    public bool locked;
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
    public string name;
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
    public string name;
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

    public event EventHandler<FNAFCreateRoomResponse> OnCreateRoomResponse;
    public event EventHandler<FNAFJoinRoomResponse> OnJoinRoomResponse;
    public event EventHandler<FNAFLoginResponse> OnLoginResponse;
    public event EventHandler<FNAFRegisterResponse> OnRegisterResponse;
    public event EventHandler<FNAFMatchmakingResponse> OnMatchmakingResponse;

    private WebSocket socket;
    private FNAFConfig config;
    private ConcurrentQueue<string> incomingMessages = new ConcurrentQueue<string>();

    private string userName;
    private string currentRoomId;

    //string oldConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"CodeStix/Net/local.json");
    private static string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"CodeStix/user.json");

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

        userName = e.name;
        config.token = e.token;
        SaveConfig();
    }

    private void FNAFClient_OnLoginResponse(object sender, FNAFLoginResponse e)
    {
        OnLoginResponse -= FNAFClient_OnLoginResponse;

        Debug.Log("Logged in: " + e.name);

        userName = e.name;
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
                OnConnected.Invoke(null, null);
                break;

            case "Disconnected":
                OnDisconnected.Invoke(null, null);
                break;

            case nameof(FNAFJoinRoomResponse):
                var joinRoomData = JsonUtility.FromJson<FNAFJoinRoomResponse>(jsonText);
                currentRoomId = joinRoomData.room.id;
                OnJoinRoomResponse.Invoke(null, joinRoomData);
                break;

            case nameof(FNAFCreateRoomResponse):
                var newRoomData = JsonUtility.FromJson<FNAFCreateRoomResponse>(jsonText);
                currentRoomId = newRoomData.room.id;
                OnCreateRoomResponse.Invoke(null, newRoomData);
                break;

            case nameof(FNAFLoginResponse):
                OnLoginResponse.Invoke(null, JsonUtility.FromJson<FNAFLoginResponse>(jsonText));
                break;

            case nameof(FNAFRegisterResponse):
                OnRegisterResponse.Invoke(null, JsonUtility.FromJson<FNAFRegisterResponse>(jsonText));
                break;

            case nameof(FNAFMatchmakingResponse):
                OnMatchmakingResponse.Invoke(null, JsonUtility.FromJson<FNAFMatchmakingResponse>(jsonText));
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

    public void RequestMatchmaking()
    {
        socket.Send(nameof(FNAFMatchmakingRequest) + ":" + JsonUtility.ToJson(new FNAFMatchmakingRequest()));
    }
}