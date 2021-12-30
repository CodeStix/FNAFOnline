using Stx.Net;
using UnityEngine;
//using Stx.Serialization;
using FNAFOnline.Shared;
//using Stx.Net.Unity;
//using Stx.Utilities;
using System;

public class NetworkSetup : MonoBehaviour
{
    public LoadingScreen loadingScreen;
    public AlertBox alertBox;
    public InputBox inputBox;
    public DangerBox dangerBox;

    void Start()
    {
        //GameSetup.RegisterNetworkTypes();

        

        //StxUnityClient.C.OnConnected += Client_OnConnected;
        //StxUnityClient.C.OnUpdateRequired += Client_OnUpdateRequired;
        //StxUnityClient.C.OnDisconnected += Client_OnDisconnected;
        //StxUnityClient.C.OnAnnouncement += Client_OnAnnouncement;
        //StxUnityClient.C.PacketCompleter = PacketCompleter;

        //StxUnityClient.Instance.Alert = alertBox.Alert;
        //StxUnityClient.Instance.InputRequired = inputBox.AskForInput;

        loadingScreen.Progress(0.6f);
        loadingScreen.Status("Connecting to the FNAF online services...");

        FNAFClient.Connect();
        FNAFClient.OnConnected += FNAFClient_OnConnected;
        FNAFClient.OnDisconnected += FNAFClient_OnDisconnected;
    }

    private void FNAFClient_OnDisconnected(object sender, WebSocketSharp.CloseEventArgs e)
    {
        FNAFClient.OnDisconnected -= FNAFClient_OnDisconnected;
        if (!dangerBox.IsShown)
            dangerBox.Show("Oopsie!\nYou got disconnected from the server!");
    }

    private void FNAFClient_OnConnected(object sender, EventArgs e)
    {
        Debug.Log("connected this");
        FNAFClient.OnConnected -= FNAFClient_OnConnected;
        loadingScreen.Progress(1f);
    }

    //private void PacketCompleter(RequestPacket forPacket, string requiredKey, Type requiredKeyType, Action<bool> submit)
    //{
    //    ThreadSafeData.Invoke(() =>
    //    {
    //        inputBox.AskForInput($"Please supply input for { requiredKey }:", "", (input) =>
    //        {
    //            if (string.IsNullOrEmpty(input)) // Input is null if it was canceled.
    //            {
    //                // We mark the request as failed.
    //                forPacket.ResponseFail();
    //                submit.Invoke(false);
    //                // We allow to cancel by returning true.
    //                return true;
    //            }

    //            object answer = null;

    //            if (requiredKeyType == typeof(string))
    //            {
    //                answer = input;
    //            }
    //            else if (requiredKeyType == typeof(int))
    //            {
    //                int i;
    //                if (int.TryParse(input, out i))
    //                    answer = i;
    //            }
    //            else
    //            {
    //                forPacket.ResponseFail();

    //                // A non supported required type was given, allow cancellation.
    //                submit.Invoke(false);
    //                return true;
    //            }

    //            if (answer != null)
    //            {
    //                forPacket.Data.Add(requiredKey, input);

    //                // We respond to the request with success.
    //                submit.Invoke(true);
    //                // Valid input was given, stop asking for input.
    //                return true;
    //            }
    //            else
    //            {
    //                // The input data was null, caused by invalid parse or input.
    //                // We do not allow the submit, try again.
    //                return false;
    //            }
    //        });
    //    });
    //}

    //private void Client_OnAnnouncement(string message)
    //{
    //    alertBox.Alert(message, "Server Announcement");
    //}

    //private void Client_OnUpdateRequired(string updateDownloadLocation)
    //{
    //    loadingScreen.Progress(1f);

    //    if (!dangerBox.IsShown)
    //        dangerBox.Show("Ow snap!\nThere is a fresh new update available, to play, you NEED to download it from the link below.", updateDownloadLocation);
    //}

    //private void Client_OnConnected(bool firstTime)
    //{
    //    loadingScreen.Progress(1f);

    //    if (firstTime)
    //    {
    //        inputBox.AskForInput("Welcome to FNAFOnline,\nplease give yourself a name:", "Your name should consist of normal alphanumeric characters, avoid geeky unnecessary symbols, and it should not be longer than 20 characters and cannot be taken yet.", (input) =>
    //        {
    //            if (!StringChecker.IsValidName(input))
    //            {
    //                return false;
    //            }

    //            StxUnityClient.F.SetName(input, (state) =>
    //            {
    //                if (state.WasSuccessful())
    //                {
    //                    alertBox.Alert($"Welcome to FNAFOnline, { input }!", "Message");
    //                }
    //            });

    //            return true;
    //        });
    //    }
    //    else
    //    {
    //        alertBox.Alert($"Welcome back { StxUnityClient.You.ShortDisplayName }!", "Message");
    //    }

    //    Invoke(nameof(Test), 2.0f);
    //}

    private void Test()
    {
        //StxUnityClient.Instance.AskForInput("Please give input:", "Enter it above /\\", (str) => false);
    }
}
