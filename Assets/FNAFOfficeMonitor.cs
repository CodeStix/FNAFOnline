using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FNAFOfficeMonitor : MonoBehaviour
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

    public void Up()
    {
        targetIndex = textures.Length - 1;
    }

    public void Down()
    {
        targetIndex = 0;
    }

}
