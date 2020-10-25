using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Static : MonoBehaviour
{
    public Sprite[] frames;
    [Range(0f, 1f)]
    public float targetAlpha = 1f;
    public float lerpSpeed = 1f;

    private Image image;
    private int index = 0;
    private float initialAlpha;

    void Start()
    {
        image = GetComponent<Image>();

        initialAlpha = image.color.a;
    }

    void OnDisable()
    {
        SetAlpha(initialAlpha);
    }

    void Update()
    {
        SetAlpha(Mathf.Lerp(image.color.a, targetAlpha, Time.deltaTime * lerpSpeed));

        image.sprite = frames[index];

        if (++index >= frames.Length)
            index = 0;
    }

    public void SetAlpha(float a)
    {
        Color c = image.color;
        c.a = a;
        image.color = c;
    }
}
