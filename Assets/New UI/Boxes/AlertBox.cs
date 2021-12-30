using Stx.Net.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animation))]
public class AlertBox : MonoBehaviour
{
    public string showAnimation;
    public string hideAnimation;
    [Space] 
    public Text messageText;
    public Text titleText;
    public Image moodImage;
    public Color positiveMood = Color.green;
    public Color negativeMood = Color.red;
    [Space]
    [Range(0.001f, 0.5f)]
    public float timePerChar = 0.03f;

    private Animation m;

    public static AlertBox Instance { get; private set; }

    void Start()
    {
        Instance = this;
        m = GetComponent<Animation>();
    }

    public void Alert(string message, string title = "Alert", bool isPositive = true)
    {
        moodImage.color = isPositive ? positiveMood : negativeMood;

        messageText.text = message;
        titleText.text = title;

        m.Play(showAnimation);

        CancelInvoke(nameof(Hide));
        Invoke(nameof(Hide), timePerChar * message.Length);
    }

    private void Hide()
    {
        m.Play(hideAnimation);
    }

    public void Close()
    {
        CancelInvoke(nameof(Hide));

        Hide();
    }
}
