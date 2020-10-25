using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FadedSprite : MonoBehaviour
{
    public float targetAlpha = 0.5f;
    public float lerpSpeed = 1f;

    private new SpriteRenderer renderer;
    private float fromAlpha = 1f;
    private float waitTime = 0;

	void Awake()
    {
        this.renderer = GetComponent<SpriteRenderer>();

        fromAlpha = renderer.color.a;
    }

    void OnEnable()
    {
        Fade();
    }

    void Update()
    {
        waitTime -= Time.deltaTime;

        if (waitTime <= 0f)
            SetAlpha(Mathf.Lerp(renderer.color.a, targetAlpha, Time.deltaTime * lerpSpeed));
    }

    public void Fade()
    {
        SetAlpha(fromAlpha);
    }

    public void FadeLong()
    {
        waitTime = 1f;
        SetAlpha(fromAlpha);
    }

    public void FadeOut()
    {
        targetAlpha = 0f;
    }

    public void FadeTo(float alpha)
    {
        targetAlpha = alpha;
    }

    private void SetAlpha(float a)
    {
        Color c = renderer.color;
        c.a = a;
        renderer.color = c;
    }
}
