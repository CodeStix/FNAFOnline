using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SecurityAI : MonoBehaviour
{
    public States officeLitLeft;
    public States officeLitRight;
    [Space]
    public States leftDoor;
    public States leftLight;
    public States rightDoor;
    public States rightLight;
    [Space]
    public States securityGuardBlibs;
    [Space]
    public SecurityTimings securityTimings;
    [Space]
    public SecurityDanger[] dangers;
    [Space]
    public GameObject goldenFreddy;
    public UnityEvent onSeeGoldenFreddy;

    private float changeStateTime = 10f;
    private float stateTime = 10f;
    private bool state = false;

    private bool shouldCloseLeftDoor = false;
    private bool shouldCloseRightDoor = false;
    private bool shouldSwitchState = false;
    private bool rightDanger = false;
    private bool leftDanger = false;
    private bool sawGoldenFreddy = false;

    void Update()
    {
        changeStateTime -= Time.deltaTime;

        if (changeStateTime <= 0f)
        {
            changeStateTime = Random.Range(securityTimings.minChangeStateTime, securityTimings.maxChangeStateTime);

            state = !state;
            shouldSwitchState = false;

            if (state)
            {
                // IN OFFICE
                securityGuardBlibs.JumpLast();
            }
            else
            {
                // IN CAMERA
                leftLight.JumpFirst();
                rightLight.JumpFirst();
                leftDanger = false;
                rightDanger = false;
                securityGuardBlibs.JumpRandom();
                if (onSeeGoldenFreddy != null && sawGoldenFreddy)
                {
                    onSeeGoldenFreddy.Invoke();
                    sawGoldenFreddy = false;
                }
            }
        }

        stateTime -= Time.deltaTime;

        if (sawGoldenFreddy)
            stateTime -= Time.deltaTime;

        if (shouldSwitchState && Random.Range(0f, 1f) <= securityTimings.stateChangeChance)
        {
            changeStateTime = 0f;
        }

        if (stateTime <= 0f)
        {
            stateTime = Random.Range(securityTimings.minReactionTime, securityTimings.maxReactionTime);

            if (state)
            {
                // IN OFFICE

                if (goldenFreddy.activeInHierarchy)
                {
                    shouldSwitchState = true;
                    sawGoldenFreddy = true;
                }

                int todo = Random.Range(0, 3);
                if (todo == 0)
                {
                    rightLight.JumpFirst();
                    leftLight.ToggleFirstSecond();

                    if (officeLitLeft.GetCurrentState() > 0 || leftDanger)
                    {
                        if (shouldCloseLeftDoor)
                        {
                            leftDoor.Jump(1);
                        }

                        shouldCloseLeftDoor = true;
                        changeStateTime += securityTimings.maxReactionTime;
                    }
                    else if (shouldCloseLeftDoor)
                    {
                        shouldCloseLeftDoor = false;

                        leftDoor.JumpFirst();
                    }
                }
                else if (todo == 1)
                {
                    leftLight.JumpFirst();
                    rightLight.ToggleFirstSecond();

                    if (officeLitRight.GetCurrentState() > 0 || rightDanger)
                    {
                        if (shouldCloseRightDoor)
                        {
                            rightDoor.Jump(1);
                        }

                        shouldCloseRightDoor = true;
                        changeStateTime += securityTimings.maxReactionTime;
                    }
                    else if (shouldCloseRightDoor)
                    {
                        shouldCloseRightDoor = false;

                        rightDoor.JumpFirst();
                    }
                }
            }
            else
            {
                // IN CAMERA
                securityGuardBlibs.JumpRandom();

                string j = securityGuardBlibs.GetCurrentStateName();

                foreach(SecurityDanger d in dangers)
                {
                    if ((d.cameraID == j || d.cameraID == "ALL") && d.dangerStatesIndexes.Contains(d.dangerState.GetCurrentState()))
                    {
                        shouldSwitchState |= d.shouldSwitchState;
                        rightDanger |= d.rightDanger;
                        leftDanger |= d.leftDanger;
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class SecurityDanger
{
    public string cameraID = "ALL";
    [Space]
    public States dangerState;
    public List<int> dangerStatesIndexes;
    [Space]
    public bool shouldSwitchState = true;
    public bool leftDanger;
    public bool rightDanger;
}

[System.Serializable]
public class SecurityTimings
{
    [Range(0.01f, 4f)]
    public float minReactionTime = 0.3f;
    [Range(0.01f, 4f)]
    public float maxReactionTime = 1.5f;
    [Range(1f, 20f)]
    public float minChangeStateTime = 3f;
    [Range(1f, 20f)]
    public float maxChangeStateTime = 8f;
    [Range(0f, 1f)]
    public float stateChangeChance = 0.5f;
}