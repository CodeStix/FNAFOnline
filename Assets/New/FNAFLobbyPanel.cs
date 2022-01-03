using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FNAFLobbyPanel : MonoBehaviour
{
    public bool hideUnusedPlayerSpots = true;
    public List<FNAFLobbyPlayer> playerSpots;
    [Space]
    public UnityEvent onJoin;
    public UnityEvent onLeave;
    public UnityEvent onReady;
    public UnityEvent onUnReady;
    [Space]
    public Text readyButtonText;
    public string readyText = "<color=#FF2200>Click if ready.</color>";
    public string notReadyText = "<color=#33FF11>You're ready.</color>";
    [Space]
    public Text roomCodeText;

    private FNAFRoom room;
    private bool ready = false;

    void OnEnable()
    {
        FNAFClient.Instance.OnRoomChangeEvent += Instance_OnRoomChangeEvent;

        room = FNAFClient.Instance.GetRoom();

        UpdatePlayersUI();
        readyButtonText.text = notReadyText;
    }

    void OnDisable()
    {
        FNAFClient.Instance.OnRoomChangeEvent -= Instance_OnRoomChangeEvent;
    }

    private void Instance_OnRoomChangeEvent(object sender, FNAFRoomChangeEvent e)
    {
        if (e.eventType == "leave")
        {
            onLeave?.Invoke();
        }
        else if (e.eventType == "join")
        {
            onJoin?.Invoke();
        }
        else if (e.eventType == "ready")
        {
            onReady?.Invoke();
        }
        else if (e.eventType == "unready")
        {
            onUnReady?.Invoke();
        }
        else if (e.eventType == "start")
        {
            LoadingScreen.LoadScene("Office1");
        }

        room = e.room;
        UpdatePlayersUI();
    }

    public void UpdatePlayersUI()
    {
        roomCodeText.text = room.id;
        for (int i = 0; i < playerSpots.Count; i++)
        {
            FNAFLobbyPlayer pl = playerSpots[i];
            if (i >= room.maxPlayers)
            {
                // Hide spot
                pl.gameObject.SetActive(false);
            }
            else
            {
                pl.gameObject.SetActive(true);
                if (i < room.users.Count)
                {
                    // Show player in spot
                    pl.UseFor(room.users[i]);
                }
                else
                {
                    // Show available empty spot
                    pl.UseFor(null);
                }
            }
        }
    }

    public void Leave()
    {
        FNAFClient.Instance.OnLeaveRoomResponse += Instance_OnLeaveRoomResponse;
        FNAFClient.Instance.LeaveRoom();
    }

    private void Instance_OnLeaveRoomResponse(object sender, FNAFLeaveRoomResponse e)
    {
        FNAFClient.Instance.OnLeaveRoomResponse -= Instance_OnLeaveRoomResponse;
        if (e.ok)
        {
            LoadingScreen.LoadScene("GameMenu");
        }
    }

    public void ReadyToggle()
    {
        FNAFClient.Instance.OnStartResponse += Instance_OnStartResponse;
        FNAFClient.Instance.StartGameRequest(!ready);
    }

    private void Instance_OnStartResponse(object sender, FNAFStartResponse e)
    {
        FNAFClient.Instance.OnStartResponse -= Instance_OnStartResponse;

        if (e.ok)
        {
            ready = e.ready;
            readyButtonText.text = e.ready ? readyText : notReadyText;
        }
    }
}
