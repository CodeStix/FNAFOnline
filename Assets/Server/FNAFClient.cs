using System;
using UnityEngine;
using WebSocketSharp;

public class FNAFClient
{
    private const string CONNECTION_URL = "ws://localhost:8080";

    public static event EventHandler OnConnected;
    public static event EventHandler<CloseEventArgs> OnDisconnected;

    private static WebSocket socket;

    public static void Connect()
    {
        OnConnected += (_a, _b) =>
        {
            Debug.Log("[FNAFClient] Connected to WebSocket");
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

    private static void Socket_OnMessage(object sender, MessageEventArgs e)
    {
        if (!e.IsText || e.IsPing) return;

        string message = e.Data;

        Debug.Log("Received message");
    }
}