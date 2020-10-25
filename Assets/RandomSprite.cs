using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RandomSprite : MonoBehaviour
{
    public bool onStart = true;
    [Space]
    public Sprite sprite;
    [Range(0f,1f)]
    public float chance = 0.5f;

    private new SpriteRenderer renderer;

    void Awake()
    {
        this.renderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (onStart)
            TryReplace();
    }

    public void TryReplace()
    {
        if (Random.Range(0f,1f) <= chance)
        {
            ForceReplace();
        }
    }

    public void ForceReplace()
    {
        if (renderer == null)
            Debug.Log("renderer is null");

        renderer.sprite = sprite;
    }
}
