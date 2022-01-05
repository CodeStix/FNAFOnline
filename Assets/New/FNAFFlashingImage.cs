using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FNAFFlashingImage : MonoBehaviour
{
    public float interval;
    public Image image;

    private float time = 0f;
    private Color color;

    private void Start()
    {
        color = image.color;
    }

    private void Update()
    {
        time += Time.deltaTime;

        if (time >= interval)
        {
            time = 0f;
            if (image.color  == color)
            {
                image.color = Color.clear;
            }
            else
            {
                image.color = color;
            }
        }
    }
}
