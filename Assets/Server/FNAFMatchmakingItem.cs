using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FNAFRoom
{
    public string Name { get; set; }
    public string OwnerDisplayName { get; set; }
    public int PlayerCount { get; set; }
    public int MaxPlayers { get; set; }
    public bool Locked { get; set; }
}

public class FNAFMatchmakingItem : MonoBehaviour
{
    public Text nameText;
    public Text ownerText;
    public Text playerCountText;
    public GameObject locked;
    public Button joinButton;

    [HideInInspector]
    public FNAFRoom room;

    public void Show()
    {
        nameText.text = room.Name;
        ownerText.text = room.OwnerDisplayName;
        playerCountText.text = $"{ room.PlayerCount }/{ room.MaxPlayers }";
        locked.SetActive(room.Locked);
    }

    public void Join()
    {
        if (room.Locked)
        {
            Debug.LogError("Locked rooms not implemented");
            return;
        }
    }
}
