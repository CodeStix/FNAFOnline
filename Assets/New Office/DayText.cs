using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DayText : MonoBehaviour
{
    public string format = "Night {0}";

    private Text text;
    private string file;

    void Start()
    {
        text = GetComponent<Text>();
        file = Path.Combine(Application.persistentDataPath, "day.txt");

        int current = Load();

        text.text = string.Format(format, current);

        current++;
        Save(current);
    }

    private void Save(int day)
    {
        try
        {
            File.WriteAllText(file, $"{ day }\n{ GenerateChecksum(day) }");
        }
        catch
        { }
    }

    private int Load()
    {
        if (File.Exists(file))
        {
            try
            {
                string[] l = File.ReadAllLines(file);

                if (l.Length <= 1)
                    return 1;

                int i = int.Parse(l[0]);
                int j = int.Parse(l[1]);

                if (j != GenerateChecksum(i))
                    return 1;

                return i;
            }
            catch
            {
                return 1;
            }
        }
        else
        {
            return 1;
        }
    }

    private int GenerateChecksum(int f)
    {
        const int key = 0x0f3782ea;

        f = (int)(f * 349.69721f);
        f -= key * (f % 109);
        f += key * (f % 42);

        f = ~f;

        byte[] b = BitConverter.GetBytes(f);
        byte[] bk = BitConverter.GetBytes(key);

        int l = 9;

        for (int i = 0; i < b.Length; i++)
        {
            b[b.Length - 1 - i] ^= bk[i];
            l -= i * (bk[i] + b[i] - 1);
        }

        f += Mathf.RoundToInt(Mathf.Sin(l * 10f) * 2390.3f);

        return f;
    }
}
