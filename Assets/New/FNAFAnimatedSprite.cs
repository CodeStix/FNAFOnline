using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FNAFAnimatedSprite : MonoBehaviour
{
    public Sprite[] textures;
    public float speed = 10;
    public bool loop = true;
    [Space]
    public Image orImage;
    public SpriteRenderer orSpriteRenderer;

    private float current = 0;

    void Start()
    {
        
    }

    void Update()
    {
        if ((int)current < textures.Length)
        {
            if (orImage != null)
                orImage.sprite = textures[(int)current];
            if (orSpriteRenderer != null)
                orSpriteRenderer.sprite = textures[(int)current];

            current += speed * Time.deltaTime;

            if((int)current >= textures.Length)
            {
                if (loop)
                {
                    current = 0;
                }
            }
        }
    }

    public void Play()
    {
        current = 0;
    }
}
