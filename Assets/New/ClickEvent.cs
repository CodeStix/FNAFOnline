using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class ClickEvent : MonoBehaviour
{
    public UnityEvent clickEvent;
    public bool canClick = true;
    public float noClickTime = 0;
    public GameObject onlyIfActive;

    private float time = 0;

    void OnEnable()
    {
        time = noClickTime;
    }

    void Update()
    {
        time -= Time.deltaTime;
    }

    void OnMouseDown()
    {
        if (!canClick)
            return;

        bool a = true;

        if (onlyIfActive != null)
            a = onlyIfActive.activeInHierarchy;

        if (time <= 0 && a)
        {
            InvokeEvents();
            time = noClickTime;
        }
    }

    public void InvokeEvents()
    {
        clickEvent.Invoke();
    }

    public void InvokeEventsIfActive()
    {
        if (gameObject.activeSelf)
            clickEvent.Invoke();
    }
}
