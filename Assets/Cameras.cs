using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Obsolete]
public class Cameras : MonoBehaviour
{
    public States flip;
    public GameObject screen;
    public UnityEvent onOpen;
    public UnityEvent onClose;

    private bool open = false;
    private bool disabled = false;

    public void Start()
    {
        screen.SetActive(false);
    }

    public void Open()
    {
        if (disabled)
            return;

        if (onOpen != null)
            onOpen.Invoke();

        open = true;
        flip.Jump(1);
        StartCoroutine(SetActiveLater(screen, true, 0.42f));
    }

    public void Close()
    {
        if (onClose != null)
            onClose.Invoke();

        open = false;
        flip.JumpLast();
        screen.SetActive(false);
    }

    private IEnumerator SetActiveLater(GameObject obj, bool active, float time)
    {
        if (disabled)
            yield break;

        yield return new WaitForSeconds(time);
        obj.SetActive(active);
    }

    public void Toggle()
    {
        if (disabled)
            return;

        if (open)
            Close();
        else
            Open();
    }

    public void Disable()
    {
        if (open)
            Close();

        disabled = true;
    }

    public void Enable()
    {
        disabled = false;
    }

}
