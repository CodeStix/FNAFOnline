using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RandomDelayEvent : MonoBehaviour
{
    public float minDelay = 1f;
    public float maxDelay = 10f;
    public bool loop = true;
    public UnityEvent afterDelayEvent;

    private float time;
    private bool invoked = false;

    void OnEnable()
    {
        time = Random.Range(minDelay,maxDelay);
        invoked = false;
    }

    void Update()
    {
        time -= Time.deltaTime;

        if (time <= 0f && !invoked)
        {
            if (loop)
            {
                time = Random.Range(minDelay, maxDelay);
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

