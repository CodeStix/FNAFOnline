using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FNAFGameSettingsPanel : MonoBehaviour
{
    public Button editRoomButton;
    public Dropdown maxPlayerDropdown;
    public Dropdown gameModeDropdown;

    public Text gameDescriptionText;
    public string[] gameDescriptions;


    void Start()
    {
        maxPlayerDropdown.onValueChanged.AddListener(OnChangeMaxPlayers);
        gameModeDropdown.onValueChanged.AddListener(OnChangeGameMode);
        UpdateUI(FNAFClient.Instance.GetRoom());
    }

    private void OnEnable()
    {
        FNAFClient.Instance.OnRoomChangeEvent += Instance_OnRoomChangeEvent;
    }

    private void OnDisable()
    {
        FNAFClient.Instance.OnRoomChangeEvent -= Instance_OnRoomChangeEvent;
    }

    private void Instance_OnRoomChangeEvent(object sender, FNAFRoomChangeEvent e)
    {
        Debug.Log("Received updated room: " + e.room);
        UpdateUI(e.room);
    }

    private void UpdateUI(FNAFRoom room)
    {
        maxPlayerDropdown.value = room.maxPlayers - 2;
        gameModeDropdown.value = room.game.gameMode == "classic" ? 0 : 1;

        editRoomButton.gameObject.SetActive(room.ownerId == FNAFClient.Instance.GetUser().id);
        gameDescriptionText.text = gameDescriptions[gameModeDropdown.value];
    }

    private void OnChangeMaxPlayers(int maxPlayersIndex)
    {
        FNAFRoom room = FNAFClient.Instance.GetRoom();

        int newValue = maxPlayersIndex + 2;
        if (newValue != room.maxPlayers)
        {
            room.maxPlayers = newValue;
            Debug.Log("Sending new room: " + room);
            FNAFClient.Instance.EditRoom(room);
        }
    }

    private void OnChangeGameMode(int gameModeIndex)
    {
        FNAFRoom room = FNAFClient.Instance.GetRoom();

        string newGameMode = gameModeIndex == 0 ? "classic" : "freeForAll";
        if (room.game.gameMode != newGameMode)
        {
            room.game.gameMode = newGameMode;
            Debug.Log("Sending new room: " + room);
            FNAFClient.Instance.EditRoom(room);
        }
    }
}
