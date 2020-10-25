using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class StatText : MonoBehaviour
{
    [TextArea]
    public string format;
    [Help("Use string formatting like this: {stat1}, {statN}...")]
    [Space]
    public bool updateLive = false;
    public float updateInterval = 1f;

    private Text text;
    private float time;

    void Start()
    {
        text = GetComponent<Text>();

        time = updateInterval;

        UpdateText();
    }

    void Update()
    {
        if (updateLive)
        {
            if (updateInterval == 0f)
            {
                UpdateText();
                return;
            }

            time -= Time.deltaTime;

            if (time <= 0f)
            {
                time = updateInterval;

                UpdateText();
            }
        }
    }

    public void UpdateText()
    {
        string newText = format;

        foreach(JStat stat in Stats.j.data.stats)
            newText = newText.Replace("{" + stat.Name + "}", stat.Value);

        text.text = newText;
    }
}
