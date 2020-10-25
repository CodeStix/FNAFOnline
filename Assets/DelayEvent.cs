using Stx.Net.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayEvent : MonoBehaviour
{
    public float delay = 1f;
    public bool loop = false;
    public UnityEvent afterDelayEvent;

    private float time;
    private bool invoked = false;

    void OnEnable()
    {
        time = delay;
        invoked = false;

        if (delay <= 0f)
        { 
            afterDelayEvent.Invoke();

            invoked = true;
        }

    }

    void Update()
    {
        time -= Time.deltaTime;

        if (time <= 0f && !invoked)
        {
            if (loop)
            {
                time = delay;
                invoked = false;
            }
            else
            {
                invoked = true;
            }

            afterDelayEvent.Invoke();
        }
    }
}