using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrizeItem : MonoBehaviour
{
    public bool unlocked = false;
    public Prize prize;
    [Space]
    public Image image;

    private PrizeCollection collection;
    //private Sprite original;

    void Start()
    {
        collection = transform.parent.GetComponent<PrizeCollection>();

        if (collection == null)
            Debug.LogError("Please add prizecollection to prizeitem parent.");

        //original = image.sprite;
    }

    void Update()
    {
        if (unlocked)
            image.sprite = prize.image;//original;
        else
            image.sprite = prize.lockedImage;
    }

    public void Clicked()
    {
        collection.Display(this);
    }
}
