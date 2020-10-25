using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveCaught : MonoBehaviour
{
    public RoomScheme scheme;
    public States securityGuardBlibs;
    public UnityEvent caughtEvent;

    void Start()
    {
        //securityGuardBlibs.GetCurrentStateName();

        scheme.OnMove += Scheme_OnMove;
        scheme.OnLeave += Scheme_OnLeave;
    }

    private void Scheme_OnMove(RoomConnection room)
    {
        if (room.roomName.Contains(securityGuardBlibs.GetCurrentStateName()))
        {
            Caught();
        }
    }

    private void Scheme_OnLeave(RoomConnection room)
    {
        if (room.roomName.Contains(securityGuardBlibs.GetCurrentStateName()))
        {
            Caught();
        }
    }

    public void Caught()
    {
        if (caughtEvent != null)
            caughtEvent.Invoke();
    }


}
