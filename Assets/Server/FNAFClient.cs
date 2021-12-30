using System;
using System.IO;
using UnityEngine;
using WebSocketSharp;

[Serializable]
public class FNAFTokenRequest
{
    public string name;
}

[Serializable]
public class FNAFTokenResponse
{
    public string token;
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

    public static event EventHandler<FNAFRoomJoinResponse> OnJoinedRoom;
    public static event EventHandler<FNAFTokenResponse> OnToken;

    private static WebSocket socket;
    private static FNAFConfig config;

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
        OnConnected += (_a, _b) =>
        {
            Debug.Log("[FNAFClient] Connected to WebSocket");

            if (config.token == null)
            {
                OnToken += FNAFClient_OnToken;
                RequestToken("Freddy");
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

    private static void FNAFClient_OnToken(object sender, FNAFTokenResponse e)
    {
        OnToken -= FNAFClient_OnToken;

        Debug.Log("Received new token: " + e.token);

        config.token = e.token;
        SaveConfig();
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
                OnJoinedRoom.Invoke(null, JsonUtility.FromJson<FNAFRoomJoinResponse>(jsonText));
                break;

            case nameof(FNAFTokenResponse):
                OnToken.Invoke(null, JsonUtility.FromJson<FNAFTokenResponse>(jsonText));
                break;

            default:
                Debug.LogWarning("Received unknown message type from server: " + type);
                break;
        }
    }

    public static void RequestToken(string name)
    {
        socket.Send(nameof(FNAFTokenRequest) + ":" + JsonUtility.ToJson(new FNAFTokenRequest() { name = name }));
    }

    public static void JoinRoom(int id)
    {
        socket.Send(nameof(FNAFRoomJoinRequest) + ":" + JsonUtility.ToJson(new FNAFRoomJoinRequest() { id = id }));
    }
}