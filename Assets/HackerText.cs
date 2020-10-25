using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class HackerText : MonoBehaviour
{
    [TextArea(5, 10)]
    public string hackerScript;
    public float speed = 20.0f;
    public int scanLines = 15;
    public bool onStart = false;

    private Text text;
    private string[] lines;

    void Start()
    {
        text = GetComponent<Text>();
        lines = hackerScript.Split('\n');

        if (onStart)
            StartCoroutine(IScanVertical(0, lines.Length));
    }

    void OnDisable()
    { 
        if (text != null)
            text.text = "";
    }

    public void ScanRandom(int length)
    {
        int start = Random.Range(0, lines.Length - length - 1);
        StartCoroutine(IScanVertical(start, start + length, 1));
    }

    private IEnumerator IScanVertical(int start, int end, int loops = int.MaxValue)
    {
        int i = start;

        for (int l = 0; l < loops; l++)
        {
            i = start;

            for (; i < end; i++)
            {
                text.text = "";

                for (int j = i; j < i + scanLines; j++)
                    text.text += lines[j % lines.Length] + "\n";

                yield return new WaitForSeconds(1f / speed);
            }
        }
        int n = 0;
        for (int m = i; m < i + scanLines; m++)
        {
            text.text = "";

            for (int j = m + n++; j < m + scanLines; j++)
                text.text += lines[j % lines.Length] + "\n";

            yield return new WaitForSeconds(1f / speed);
        }

        text.text = "";
    }
	
}
