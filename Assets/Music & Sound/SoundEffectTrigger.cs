using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectTrigger : MonoBehaviour
{
    public string startSound = null;
    public string enableSound = null;
    public string disableSound = null;

    void Start()
    {
        if (!string.IsNullOrWhiteSpace(startSound))
            SoundEffects.Play(startSound);
    }

    void OnEnable()
    {
        if (!string.IsNullOrWhiteSpace(enableSound))
            SoundEffects.Play(enableSound);
    }

    void OnDisable()
    {
        if (!string.IsNullOrWhiteSpace(disableSound))
            SoundEffects.Play(disableSound);
    }

    public void Play(string sound)
    {
        SoundEffects.Play(sound);
    }

    public void PlayIfActive(string sound)
    {
        if (gameObject.activeSelf)
            SoundEffects.Play(sound);
    }

    public void Stop(string sound)
    {
        SoundEffects.Stop(sound);
    }
}