using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FNAFMatchmakingItem : MonoBehaviour
{
    public Text nameText;
    public Text ownerText;
    public Text shortDescriptionText;
    public Text playerCountText;
    public GameObject locked;
    public Button joinButton;

    [HideInInspector]
    public FNAFRoom room;

    public void Show()
    {
        nameText.text = room.name;
        ownerText.text = room.ownerName;
        shortDescriptionText.text = room.shortDescription;
        playerCountText.text = $"{ room.playerCount }/{ room.maxPlayers }";
        locked.SetActive(room.isPrivate);
    }

    public void Join()
    {
        FNAFClient.Instance.OnJoinRoomResponse += Instance_OnJoinResponse;
        FNAFClient.Instance.JoinRoom(room.id);
    }

    private void Instance_OnJoinResponse(object sender, FNAFJoinRoomResponse e)
    {
        FNAFClient.Instance.OnJoinRoomResponse -= Instance_OnJoinResponse;
        LoadingScreen.LoadScene("Lobby");
    }
}
