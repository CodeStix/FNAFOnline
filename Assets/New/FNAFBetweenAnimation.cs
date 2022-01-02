using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FNAFBetweenAnimation : MonoBehaviour
{
    public float speed = 5f;
    public Sprite[] textures;
    [Space]
    public SpriteRenderer spriteRenderer;
    public Image orImage; 

    private float index = 0;
    private int targetIndex = 0;

    private void Update()
    {
        if (index <= targetIndex)
        {
            index += speed * Time.deltaTime;
            if (index >= textures.Length)
                index = textures.Length - 1;
            UpdateImage();
        }

        if (index >= targetIndex)
        {
            index -= speed * Time.deltaTime;
            if (index < 0)
                index = 0;
            UpdateImage();
        }
    }

    private void UpdateImage()
    {
        if (orImage)
            orImage.sprite = textures[(int)index];
        if (spriteRenderer != null)
            spriteRenderer.sprite = textures[(int)index];
    }

    public void Start()
    {
        targetIndex = 0;
    }

    public void End()
    {
        targetIndex = textures.Length - 1;
    }

}
