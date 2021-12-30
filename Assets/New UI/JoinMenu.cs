using Stx.Net.Unity;
using Stx.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Stx.Net.RoomBased;
using UnityEngine.SceneManagement;

public class JoinMenu : MonoBehaviour
{
    public int playerCount = 4;
    public bool hidden = false;
    [Header("Random")]
    public Button joinRandomButton;
    [Header("With Code")]
    public InputField roomCodeInput;
    [Space]
    public Color uncompleteColor = Color.red;
    public Color completeColor = Color.green;

    private bool didTryJoin = false;

    public void JoinRandom()
    {
        joinRandomButton.interactable = false;

        FNAFClient.Instance.OnJoinRoomResponse += Instance_OnJoinRoomResponse_Random;
        FNAFClient.Instance.JoinRoom(null);
    }

    private void Instance_OnJoinRoomResponse_Random(object sender, FNAFJoinRoomResponse e)
    {
        FNAFClient.Instance.OnJoinRoomResponse -= Instance_OnJoinRoomResponse_Random;
        joinRandomButton.interactable = true;
        if (e.ok)
        {
            LoadingScreen.LoadScene("Lobby");
        }
        else
        {
            AlertBox.Instance.Alert("Could not join random room.", "Join random", false);
        }
    }

    public void JoinNew(bool isPrivate)
    {
        FNAFClient.Instance.OnCreateRoomResponse += Instance_OnCreateRoomResponse;
        FNAFClient.Instance.CreateRoom(2, isPrivate);
    }

    private void Instance_OnCreateRoomResponse(object sender, FNAFCreateRoomResponse e)
    {
        FNAFClient.Instance.OnCreateRoomResponse -= Instance_OnCreateRoomResponse;
        if (e.ok)
        {
            LoadingScreen.LoadScene("Lobby");
        }
        else
        {
            AlertBox.Instance.Alert("Could not create room.", "Create", false);
        }
    }

    void Start()
    {
        joinRandomButton.onClick.AddListener(JoinRandom);

        roomCodeInput.onValueChanged.AddListener((str) =>
        {
            str = str.Trim();

            roomCodeInput.textComponent.color = str.Length == 4 ? completeColor : uncompleteColor;

            if (!didTryJoin && str.Length == 4)
            {
                didTryJoin = true;
                roomCodeInput.interactable = false;

                FNAFClient.Instance.OnJoinRoomResponse += Instance_OnJoinRoomResponse_Input;
                FNAFClient.Instance.JoinRoom(str);
            }
            else
            {
                didTryJoin = false;
            }
        });
    }

    private void Instance_OnJoinRoomResponse_Input(object sender, FNAFJoinRoomResponse e)
    {
        FNAFClient.Instance.OnJoinRoomResponse -= Instance_OnJoinRoomResponse_Input;
        roomCodeInput.interactable = true;
        if (e.ok)
        {
            Debug.Log("Joining room " + e.room.id);
            LoadingScreen.LoadScene("Lobby");
        }
        else
        {
            AlertBox.Instance.Alert("Room not found.", "Join with code", false);
        }
    }
}
