using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyBind : MonoBehaviour
{
    public KeyCode key;
    public UnityEvent keyEvent;
    public float cooldown = 0f;

    private float time = 0f;

    void Update()
    {
        time -= Time.deltaTime;

        if (time <= 0f && Input.GetKeyDown(key))
        {
            time = cooldown;

            if (keyEvent != null)
                keyEvent.Invoke();
        }
    }

}
