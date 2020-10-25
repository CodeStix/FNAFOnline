using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class HoverEvent : MonoBehaviour
{
    public UnityEvent hoverEvent;
    public float noHoverTime = 0;

    private float time = 0;
    private bool over = false;

    void OnEnable()
    {
        time = noHoverTime;
    }

    void Update()
    {
        time -= Time.deltaTime;
    }

    void OnMouseOver()
    {
        if (!over)
        {
            if (time <= 0)
            {
                over = true;

                InvokeEvents();
                time = noHoverTime;
            }
        }
    }

    void OnMouseExit()
    {
        over = false;
    }

    public void InvokeEvents()
    {
        hoverEvent.Invoke();
    }
}
