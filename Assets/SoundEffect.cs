using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[Obsolete("Use SoundEffectTrigger instead.")]
public class SoundEffect : MonoBehaviour, IPlayable
{

    public AudioClip clip;
    [Range(0f,1f)]
    public float volume = 1f;
    public bool playOnStart = true;
    public float playOnStartDelay = 0f;
    public bool loop = true;
    public bool randomStartTime = false;

    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
    }

    void OnEnable()
    {
        source.clip = clip;
        source.loop = loop;

        if (randomStartTime)
            source.time = UnityEngine.Random.Range(0f, source.clip.length);

        if (playOnStart)
        {
            if (playOnStartDelay <= 0f)
                Play();
            else
                StartCoroutine(IPlayLater(playOnStartDelay));
        }
            
    }

    public IEnumerator IPlayLater(float playOnStartDelay)
    {
        yield return new WaitForSeconds(playOnStartDelay);
        Play();
    }

    void Update()
    {
        source.volume = volume;
    }

    public void Play()
    {
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }
}
