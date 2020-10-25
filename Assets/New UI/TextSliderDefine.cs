using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextSliderDefine : MonoBehaviour
{
    public Slider slider;
    public string format = "Value is {0}";
    public SliderDefine[] explanations;

    private Text text;
	
	void Start ()
    {
        text = GetComponent<Text>();

        UpdateText(slider.value);

        slider.onValueChanged.AddListener(UpdateText);

    }

    public void UpdateText(float newValue)
    {
        foreach(SliderDefine d in explanations)
        {
            if (d.value == newValue)
            {
                text.text = d.explanation;
                return;
            }
        }

        text.text = string.Format(format, newValue);
    }

}

[System.Serializable]
public class SliderDefine
{
    public float value;
    public string explanation;
}