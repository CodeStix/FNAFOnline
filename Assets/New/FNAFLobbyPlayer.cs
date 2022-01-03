using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FNAFLobbyPlayer : MonoBehaviour
{
    [Header("UI Items")]
    public Image avatarImage;
    public Text nameText;
    public Text nightText;
    public Button addFriendButton;

    public Text statusText;
    public Button kickButton;
    public Image ownerImage;
    [Space]
    public string readyText = "<color=#33FF11>Is Ready!</color>";
    public string notReadyText = "<color=#FF5555>Not Ready.</color>";
    public string nobodyText = "<color=#9999BB><i>Waiting...</i></color>";
    [Space]
    public Sprite defaultAvatar;
    public Sprite nobodyAvatar;

    private FNAFRoomUser user;

    public void UseFor(FNAFRoomUser user)
    {
        this.user = user;
        if (user != null)
        {
            FNAFRoom currentRoom = FNAFClient.Instance.GetRoom();
            bool currentIsOwner = currentRoom.ownerId == FNAFClient.Instance.GetUser().id;
            avatarImage.sprite = defaultAvatar;
            nameText.text = user.user.name;
            nameText.enabled = true;
            nightText.enabled = true;
            nightText.text = $"<color=#eeee00>Night {user.user.night}</color>";
            statusText.text = user.ready ? readyText : notReadyText;
            ownerImage.enabled = currentRoom.ownerId == user.user.id;
            kickButton.enabled = true;
            kickButton.interactable = currentIsOwner;
            addFriendButton.enabled = true;
            addFriendButton.interactable = false;
        }
        else
        {
            avatarImage.sprite = nobodyAvatar;
            nameText.enabled = false;
            nightText.enabled = false;
            statusText.text = nobodyText;
            ownerImage.enabled = false;
            kickButton.enabled = false;
            addFriendButton.enabled = false;
        }
    }

    public void Kick()
    {
        // TODO kick player
    }
}
