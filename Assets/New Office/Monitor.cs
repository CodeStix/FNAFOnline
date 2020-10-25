using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animation))]
public class Monitor : MonoBehaviour
{
    [Header("Monitor")]
    public string openAnimation;
    public string closeAnimation;
    [Space]
    public string openCloseSound;
    [Range(0f, 20f)]
    public float startupTime = 0f;
    public string startingUpSound;
    public string startupSound;
    public UnityEvent onStartup;


    private Animation m;
    private bool isOpen = false;
    private bool everOpened = false;

    void Start()
    {
        m = GetComponent<Animation>();
    }

    public void Open()
    {
        if (isOpen)
            return;

        if (!everOpened)
        {
            if (startupTime > 0f)
                StartCoroutine(IStartup());
            else
                onStartup?.Invoke();

            everOpened = true;
        }

        isOpen = true;

        m.Play(openAnimation);

        SoundEffects.Play(openCloseSound);
    }

    public void Close()
    {
        if (!isOpen)
            return;

        isOpen = false;

        m.Play(closeAnimation);

        SoundEffects.Play(openCloseSound);
    }

    public void Toggle()
    {
        if (isOpen)
            Close();
        else
            Open();
    }

    public bool GetIsOpen()
    {
        return isOpen;
    }

    private IEnumerator IStartup()
    {
        const float BeepInterval = 0.75f;

        for(int i = 0; i < startupTime / BeepInterval; i++)
        {
            if (isOpen)
                SoundEffects.Play(startingUpSound);

            yield return new WaitForSeconds(BeepInterval);
        }

        while (!isOpen)
        {
            yield return null;
        }

        SoundEffects.Play(startupSound);

        onStartup?.Invoke();
    }
}
