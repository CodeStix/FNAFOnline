using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class AutoState : MonoBehaviour
{
    public AutoStateEvent stateEvent;
    public float eventDelay = 1f;
    public int eventArgument = 0;

    private States states;
    private float time = 0;
    private bool invoked = false;
	
    void Awake()
    {
        states = transform.parent.GetComponent<States>();
    }

    void OnEnable()
    {
        time = eventDelay;
        invoked = false;
    }

    void Update()
    {
        time -= Time.deltaTime;

        if (time <= 0 && !invoked)
        {
            invoked = true;
            InvokeEvent();
        }
    }

    private void InvokeEvent()
    {
        if (stateEvent == AutoStateEvent.Next)
        {
            states.Next();
        }
        else if (stateEvent == AutoStateEvent.Previous)
        {
            states.Previous();
        }
        else if (stateEvent == AutoStateEvent.JumpFirst)
        {
            states.JumpFirst();
        }
        else if (stateEvent == AutoStateEvent.JumpLast)
        {
            states.JumpLast();
        }
        else if (stateEvent == AutoStateEvent.Jump)
        {
            states.Jump(eventArgument);
        }
    }

}

public enum AutoStateEvent
{
    Next,
    Previous,
    JumpFirst,
    JumpLast,
    Jump
}