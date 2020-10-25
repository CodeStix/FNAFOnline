using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class InputFieldSaver : MonoBehaviour
{
    private InputField input;
    private string file;

    private static Dictionary<string, string> savedInputs;

    void Start()
    {
        input = GetComponent<InputField>();

        file = Path.Combine(Application.persistentDataPath, "inputs.txt");

        input.onEndEdit.AddListener((str) =>
        {
            Save();
        });

        if (savedInputs == null)
            Load();

        if (savedInputs.ContainsKey(GetInstanceID().ToString()))
            input.text = savedInputs[GetInstanceID().ToString()];
    }

    private void Save()
    {
        Debug.Log($"Saving inputs to '{ file }'...");

        if (!savedInputs.ContainsKey(GetInstanceID().ToString()))
            savedInputs.Add(GetInstanceID().ToString(), input.text);
        else
            savedInputs[GetInstanceID().ToString()] = input.text;

        StringBuilder str = new StringBuilder();

        foreach(string i in savedInputs.Keys)
            str.AppendLine($"{ i }:{ savedInputs[i] }");

        try
        {
            File.WriteAllText(file, str.ToString());
        }
        catch
        {
            Debug.LogWarning($"Could not save inputs.");
        }
    }

    private void Load()
    {
        Debug.Log($"Loading inputs from '{ file }'...");

        savedInputs = new Dictionary<string, string>();

        string[] contents;

        try
        {
            contents = File.ReadAllLines(file);
        }
        catch
        {
            Debug.LogWarning($"Could not load inputs.");

            return;
        }

        foreach(string line in contents)
        {
            string[] s = line.Split(':');

            if (s.Length <= 1)
                continue;

            savedInputs.Add(s[0], s[1]);
        }
    }
}
