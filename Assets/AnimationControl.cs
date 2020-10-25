using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
[RequireComponent(typeof(Animation))]
public class AnimationControl : MonoBehaviour
{
    private new Animation animation;

    void Start()
    {
        animation = GetComponent<Animation>();
    }

    public void Play()
    {
        animation.Play();
    }
}
