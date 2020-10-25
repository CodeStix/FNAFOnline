using FNAFOnline.Shared;
using Stx.Net;
using Stx.Net.Unity;
using Stx.Net.RoomBased;
using Stx.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NightSetup : MonoBehaviour
{
    public Night night;
    public MoveTimer moveTimer;
    [Header("Guard/afton setup")]
    public GameObject[] toDisableIfSecurity;
    public MonoBehaviour[] componentsToDisableIfSecurity;
    [Space]
    public GameObject[] toDisableIfNotSecurity;
    public MonoBehaviour[] componentsToDisableIfNotSecurity;
    [Space]
    public States[] doorsLightsButtons;
    [Space]
    public GameObject[] toDisableIfObserver;
    public MonoBehaviour[] componentsToDisableIfObserver;
    public UnityEvent onIsObserver;
    [Space]
    public ClickEvent[] cameraButtons;
    /*[Header("AI")]
    public MoveAI moveAI;
    [Space]
    public SecurityAI securityAI;
    [Space]
    public SecurityTimings easySecurityTimings;
    public SecurityTimings mediumSecurityTimings;
    public SecurityTimings hardSecurityTimings;
    public SecurityTimings veryHardSecurityTimings;
    public SecurityTimings expertSecurityTimings;*/

    public static bool hasPlayed = false;
    public static bool isGuard = false;
    public static bool isAfton = false;
    public static bool isObserver = false;

    private bool hasBeenSetup = false;

    void OnEnable()
    {
        hasPlayed = true;

        Debug.Log("## Requesting setup...");

        StxUnityClient.F.RequestAsync<GameSetup>("FNAFRequireSetup", (state, e) =>
        {
            Debug.Log("## Received setup. " + state);

            if (!hasBeenSetup && state == PacketResponseStatus.Responded)
            {
                isGuard = StxUnityClient.C.NetworkID == e.GuardClientID;
                isAfton = StxUnityClient.C.NetworkID == e.AftonClientID;
                isObserver = !isAfton && !isGuard;

                Debug.Log("## isGuard: " + isGuard);
                Debug.Log("## isAfton: " + isAfton);
                Debug.Log("## isObserver: " + isObserver);
                Debug.Log("## e.GuardClientID: " + e.GuardClientID);
                Debug.Log("## e.AftonClientID: " + e.AftonClientID);
                Debug.Log("## StxUnityClient.C.NetworkID: " + StxUnityClient.C.NetworkID);

                night.usePower = isGuard;
                night.nightSeconds += e.Overtime * night.secondsPerHour;
                night.startingPower = e.StartingPower;
                moveTimer.timings = e.MoveTimerTimings;

                if (isGuard)
                {
                    foreach (GameObject obj in toDisableIfSecurity)
                        obj.SetActive(false);
                    foreach (MonoBehaviour mono in componentsToDisableIfSecurity)
                        mono.enabled = false;
                }

                if (isAfton)
                {
                    foreach (GameObject obj in toDisableIfNotSecurity)
                        obj.SetActive(false);
                    foreach (MonoBehaviour mono in componentsToDisableIfNotSecurity)
                        mono.enabled = false;

                    foreach (States s in doorsLightsButtons)
                        s.Jump(2);

                    night.powerDrainSpeed = 0f;
                }

                if (isObserver)
                {
                    foreach (GameObject obj in toDisableIfObserver)
                        obj.SetActive(false);
                    foreach (MonoBehaviour mono in componentsToDisableIfObserver)
                        mono.enabled = false;

                    foreach (States s in doorsLightsButtons)
                        s.Jump(2);

                    foreach (ClickEvent ce in cameraButtons)
                        ce.canClick = false;

                    onIsObserver?.Invoke();
                }

                hasBeenSetup = true;
            }
        });


        // AI, do not delete.
        /*if (LocalData.i.data.ContainsKey("EnableMoveAI"))
            moveAI.gameObject.SetActive(LocalData.i.GetBool("EnableMoveAI"));
        else
            moveAI.gameObject.SetActive(false);


        if (LocalData.i.data.ContainsKey("EnableSecurityAI"))
            securityAI.gameObject.SetActive(LocalData.i.GetBool("EnableSecurityAI"));
        else
            securityAI.gameObject.SetActive(false);

        if (LocalData.i.data.ContainsKey("SecurityAILevel"))
        {
            int level = 0;
            int.TryParse(LocalData.i.GetString("SecurityAILevel"), out level);

            if (level == 1)
            {
                securityAI.securityTimings = easySecurityTimings;
            }
            else if (level == 2)
            {
                securityAI.securityTimings = mediumSecurityTimings;
            }
            else if (level == 3)
            {
                securityAI.securityTimings = hardSecurityTimings;
            }
            else if (level == 4)
            {
                securityAI.securityTimings = veryHardSecurityTimings;
            }
            else if (level == 5)
            {
                securityAI.securityTimings = expertSecurityTimings;
            }
        }*/
    }
}