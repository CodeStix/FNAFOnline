using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FNAFAnimatedSprite : MonoBehaviour
{
    public Sprite[] textures;
    public float speed = 10;
    public bool loop = true;
    public bool shuffle = false;
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
                    
                    if (shuffle)
                    {
                        // Shuffle textures
                        for(int i = 0; i < textures.Length; i++)
                        {
                            Sprite t = textures[i];
                            int newIndex = Random.Range(0, textures.Length);
                            Sprite temp = textures[newIndex];
                            textures[newIndex] = t;
                            textures[i] = temp;
                        }
                    }
                }
            }
        }
    }

    public void Play()
    {
        current = 0;
    }
}
