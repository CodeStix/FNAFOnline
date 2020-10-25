using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Prize")]
public class Prize : ScriptableObject
{
    public new string name = "prize name";
    public Sprite image;
    public Sprite lockedImage;
    public Color color;
    [TextArea]
    public string information;
    [TextArea]
    public string lockedInformation = "?????";
    [Range(1, 1000)]
    public int value = 10;
    [Space]
    public string statName = "stat";
    public int minStatGrantValue = 100;

    public void ShowToUI(Image image, Image imageBorder, Text textInfo, Text textValue, bool unlocked = true)
    {
        image.sprite = unlocked ? this.image : this.lockedImage;
        imageBorder.color = this.color;
        textInfo.text = unlocked ? this.information : this.lockedInformation;
        textValue.text = "Reward: " + this.value;
    }

    public bool Granted()
    {
        return Stats.j.Exceeded(statName, minStatGrantValue);
    }
}
