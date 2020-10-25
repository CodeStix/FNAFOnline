﻿using Stx.Net.Unity;
using Stx.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Stx.Net.RoomBased;

public class JoinMenu : MonoBehaviour
{
    [Header("Random")]
    public Button joinRandomButton;
    [Header("With Code")]
    public InputField roomCodeInput;
    [Space]
    public Color uncompleteColor = Color.red;
    public Color completeColor = Color.green;

    private bool didTryJoin = false;

    void Start()
    {
        joinRandomButton.onClick.AddListener(() =>
        {
            joinRandomButton.interactable = false;

            StxUnityClient.F.JoinRandomRoomAsync(MatchmakingQuery.MatchAll, (state, r) =>
            {
                joinRandomButton.interactable = true;

                if (state.WasSuccessful())
                    StxUnityClient.Instance.SceneSwitchLobby();
                else
                    StxUnityClient.Instance.DisplayAlert($"Could not join random room.\nTry creating one.", "Joining Problem", false);
            }, new RoomTemplate(4));
        });

        roomCodeInput.onValueChanged.AddListener((str) =>
        {
            str = str.Trim();

            roomCodeInput.textComponent.color = str.Length == 4 ? completeColor : uncompleteColor;

            if (!didTryJoin && str.Length == 4)
            {
                didTryJoin = true;
                roomCodeInput.interactable = false;

                StxUnityClient.F.JoinRoomWithCodeAsync(str, (state, r) =>
                {
                    roomCodeInput.interactable = true;

                    if (state.WasSuccessful())
                        StxUnityClient.Instance.SceneSwitchLobby();
                    else
                        StxUnityClient.Instance.DisplayAlert($"Could not join room with code { str },\ntry joining another one.", "Joining Problem", false);
                });
            }
            else
            {
                didTryJoin = false;
            }
        });
    }
}
