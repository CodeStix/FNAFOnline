using System;
using System.IO;
using UnityEngine;
using WebSocketSharp;

[Serializable]
public class FNAFRoom
{
    public int id;
    public string name;
    public string ownerName;
    public int playerCount;
    public int maxPlayers;
    public bool locked;
}

[Serializable]
public class FNAFMatchmakingRequest
{
    public string token;
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
public class FNAFRoomJoinRequest
{
    public string token;
    public int id;
}

[Serializable]
public class FNAFRoomJoinResponse
{
    public bool ok;
}

[Serializable]
public class FNAFConfig
{
    public int version;
    public long createdTimestamp;
    public string token;
}

public class FNAFClient
{
    private const string CONNECTION_URL = "ws://localhost:8080";

    public static event EventHandler OnConnected;
    public static event EventHandler<CloseEventArgs> OnDisconnected;

    public static event EventHandler<FNAFRoomJoinResponse> OnJoinResponse;
    public static event EventHandler<FNAFLoginResponse> OnLoginResponse;
    public static event EventHandler<FNAFRegisterResponse> OnRegisterResponse;
    public static event EventHandler<FNAFMatchmakingResponse> OnMatchmakingResponse;

    private static WebSocket socket;
    private static FNAFConfig config;

    private static string userName;

    //string oldConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"CodeStix/Net/local.json");
    private static string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"CodeStix/user.json");

    private static void LoadConfig()
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

    private static void SaveConfig()
    {
        File.WriteAllText(configPath, JsonUtility.ToJson(config));
    }

    public static void Connect()
    {
        LoadConfig();

        OnConnected += (_a, _b) =>
        {
            Debug.Log("[FNAFClient] Connected to WebSocket");

            if (config.token == null)
            {
                OnRegisterResponse += FNAFClient_OnRegisterResponse; ;
                RegisterRequest("Freddy");
            }
            else
            {
                OnLoginResponse += FNAFClient_OnLoginResponse; ;
                LoginRequest(config.token);
            }
        };
        OnDisconnected += (_a, _b) =>
        {
            Debug.LogWarning("[FNAFClient] Disconnected from WebSocket");
        };

        socket = new WebSocket(CONNECTION_URL);
        socket.OnOpen += (sender, e) => OnConnected.Invoke(sender, e);
        socket.OnClose += (sender, e) => OnDisconnected.Invoke(sender, e);
        socket.OnMessage += Socket_OnMessage;
        socket.OnError += (_, e) =>
        {
            Debug.LogError("[FNAFClient] Error: " + e.Message);
        };

        socket.ConnectAsync();
    }

    private static void FNAFClient_OnRegisterResponse(object sender, FNAFRegisterResponse e)
    {
        OnRegisterResponse -= FNAFClient_OnRegisterResponse;

        Debug.Log("Registered new user: " + e.token);

        userName = e.name;
        config.token = e.token;
        SaveConfig();
    }

    private static void FNAFClient_OnLoginResponse(object sender, FNAFLoginResponse e)
    {
        OnLoginResponse -= FNAFClient_OnLoginResponse;

        Debug.Log("Logged in: " + e.name);

        userName = e.name;
    }

    private static void Socket_OnMessage(object sender, MessageEventArgs e)
    {
        if (!e.IsText || e.IsPing) return;

        string message = e.Data;
        int splitIndex = message.IndexOf(':');
        
        if (splitIndex < 0)
        {
            Debug.LogError("Received invalid message from server: " + message);
            return;
        }

        string type = message.Substring(0, splitIndex);
        string jsonText = message.Substring(splitIndex + 1);

        Debug.Log("Received message of type " + type);

        switch (type)
        {
            case nameof(FNAFRoomJoinResponse):
                OnJoinResponse.Invoke(null, JsonUtility.FromJson<FNAFRoomJoinResponse>(jsonText));
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

    public static void RegisterRequest(string name)
    {
        socket.Send(nameof(FNAFRegisterRequest) + ":" + JsonUtility.ToJson(new FNAFRegisterRequest() { name = name }));
    }

    public static void LoginRequest(string token)
    {
        socket.Send(nameof(FNAFLoginRequest) + ":" + JsonUtility.ToJson(new FNAFLoginRequest() { token = token }));
    }

    public static void JoinRoom(int id)
    {
        socket.Send(nameof(FNAFRoomJoinRequest) + ":" + JsonUtility.ToJson(new FNAFRoomJoinRequest() { id = id, token = config.token }));
    }

    public static void RequestMatchmaking()
    {
        socket.Send(nameof(FNAFMatchmakingRequest) + ":" + JsonUtility.ToJson(new FNAFMatchmakingRequest() { token = config.token }));
    }
}