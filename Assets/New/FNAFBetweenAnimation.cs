using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FNAFBetweenAnimation : MonoBehaviour
{
    public float speed = 5f;
    public SpriteRenderer spriteRenderer;
    public Sprite[] textures;

    private float index = 0;
    private int targetIndex = 0;

    private void Update()
    {
        if (index < targetIndex)
        {
            index += speed * Time.deltaTime;
            spriteRenderer.sprite = textures[(int)index];
        }
        if (index > targetIndex)
        {
            index -= speed * Time.deltaTime;
            spriteRenderer.sprite = textures[(int)index];
        }
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
