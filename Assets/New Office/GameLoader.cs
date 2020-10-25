using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stx.Net;
using Stx.Net.Unity;
using UnityEngine.UI;
using FNAFOnline.Shared;

public class GameLoader : MonoBehaviour
{
    [Header("Gamemodes")]
    public OriginalFNAF1Game originalFNAF1;
    [Space]
    public LoadingScreen waitingForOthersLoadingScreen;
    public GameObject nightStartObject;

    void Awake()
    {
        originalFNAF1.gameObject.SetActive(false);

        nightStartObject.SetActive(false);
        waitingForOthersLoadingScreen.gameObject.SetActive(true);
    }

    void Start()
    {
        StxUnityClient.C.DataReceiver.AddHandler(new DataHandler<GameSetup>("FNAFStart", OnFNAFStart));

        waitingForOthersLoadingScreen.Progress(0.69f);
        waitingForOthersLoadingScreen.Status("Waiting for other players...");

        // Send that this clients scene has loaded.
        StxUnityClient.F.RequestAsync("FNAFLoaded");
    }

    void OnDisable()
    {
        StxUnityClient.C.DataReceiver.RemoveHandler("FNAFStart");
    }

    private void OnFNAFStart(GameSetup gameSetup, IDataHolder data)
    {
        // Everyones client has loaded the scene

        waitingForOthersLoadingScreen.Progress(1f);
        waitingForOthersLoadingScreen.Status("Everyone is ready!");

        StartCoroutine(IStartNight(gameSetup));
    }

    private IEnumerator IStartNight(GameSetup gameSetup)
    {
        nightStartObject.SetActive(true);

        yield return new WaitForSeconds(5f);

        nightStartObject.SetActive(false);

        // if (gameSetup.GameMode == GameMode.OriginalFNAF1)
        // Set the active map to FNAF 1.
        if (gameSetup.GameMode == GameMode.OriginalFNAF1)
        {
            originalFNAF1.gameSetup = gameSetup;
            originalFNAF1.gameObject.SetActive(true);
        }
    }
}