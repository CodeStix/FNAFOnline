//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AnimatedImage : MonoBehaviour, IPlayable
{
    public new bool enabled = true;
    public Sprite[] sprites;
    public float delay = 0.5f;
    public SortingType sort;
    public bool invertOrder = false;
    public bool burstOnStart = true;
    public int burstLoop = 1;

    public delegate void OnBurstCompleteDelegate();

    private new Image renderer;
    private float time = 0f;
    private int current = 0;
    private int burst = 0;
    private OnBurstCompleteDelegate onBurstComplete;

    void Awake()
    {
        renderer = GetComponent<Image>();
    }

    void OnEnable()
    {
        if (burstOnStart)
            PlayBurst();
	}
	
	void Update()
    {
        if (!enabled)
        {
            renderer.sprite = sprites[0];
            return;
        }

        time -= Time.deltaTime;

        if (time <= 0)
        {
            time = delay;

            if (sort == SortingType.Ordered || (sort == SortingType.OrderedBurst && burst > 0))
            {
                if (!invertOrder)
                    renderer.sprite = sprites[current++];
                else
                    renderer.sprite = sprites[sprites.Length - current++ - 1];

                if (current >= sprites.Length)
                {
                    current = 0;

                    if (sort == SortingType.OrderedBurst)
                    {
                        burst--;

                        if (burst <= 0)
                        {
                            if (onBurstComplete != null)
                            {
                                onBurstComplete.Invoke();
                                onBurstComplete = null;
                            }
                        }
                    }
                }
            }
            else if (sort == SortingType.Random)
            {
                renderer.sprite = sprites[Random.Range(0, sprites.Length)];
            }
        }
	}

    public void PlayBurst()
    {
        burst = burstLoop;
    } 

    public void PlayBurst(int burstLoop)
    {
        burst = burstLoop;
    }

    public void PlayBurst(OnBurstCompleteDelegate onComplete)
    {
        burst = burstLoop;
        onBurstComplete = onComplete;
    }

    public void PlayBurst(int burstLoop, OnBurstCompleteDelegate onComplete)
    {
        burst = burstLoop;
        onBurstComplete = onComplete;
    }

    public void EnableOnlyMeHere()
    {
        enabled = true;

        for(int i = 0; i < transform.parent.childCount; i++)
        {
            Transform t = transform.parent.GetChild(i);

            if (t == transform)
                continue;

            AnimatedSprite a = t.GetComponent<AnimatedSprite>();
            if (a != null)
                a.enabled = false;
        }
    }

    void IPlayable.Play()
    {
        PlayBurst();
    }
}