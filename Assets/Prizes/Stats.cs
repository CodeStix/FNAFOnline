using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Stats : MonoBehaviour
{
    public bool writeEnabled = true;
    public string jsonDataFile = "prizecorner.json";

    //private JStats stats;
    private string filePath;

    public static Stats i = null;
    public static JConfig<JStat> j = null;

    void Awake()
    {
        if (i != null)
        {
            Destroy(gameObject);
            return;
        }
        i = this;

        filePath = Path.Combine(Application.persistentDataPath, jsonDataFile);

        j = new JConfig<JStat>(filePath);

        j.Increment("startups", 1);
    }

    

    
}

public class JConfig<T> where T : IKeyValuePair<string, string>, new()
{
    public string fileName;
    public JNamedList<T> data;
    public bool writeEnabled = true;

    public JConfig(string fileName)
    {
        this.fileName = fileName;

        if (File.Exists(fileName))
        {
            data = JsonUtility.FromJson<JNamedList<T>>(File.ReadAllText(fileName));
        }
        else
        {
            data = new JNamedList<T>();

            Save();
        }
    }

    public void Put(string name, object value)
    {
        if (!writeEnabled)
            return;

        if (data.Contains(name))
        {
            T s = data.Get(name);

            s.Value = value.ToString();
        }
        else
        {
            T n = new T();
            n.Name = name;
            n.Value = value.ToString();

            data.Add(n);
        }

        Save();
    }

    public int GetInt(string name, int defaultValue = 0)
    {
        if (data.Contains(name))
        {
            int s = 0;
            int.TryParse(data.Get(name).Value, out s);
            return s;
        }
        else
        {
            return defaultValue;
        }
    }

    public bool GetBool(string name, bool defaultValue = false)
    {
        if (data.Contains(name))
        {
            bool s = defaultValue;
            bool.TryParse(data.Get(name).Value, out s);
            return s;
        }
        else
        {
            return defaultValue;
        }
    }

    public void Increment(string name, int value = 1)
    {
        Put(name, GetInt(name) + value);
    }

    public bool Exceeded(string name, int value)
    {
        return GetInt(name) >= value;
    }

    public void Save()
    {
        File.WriteAllText(fileName, JsonUtility.ToJson(data));
    }
}

[System.Serializable]
public class JNamedList<T> where T : IKeyValuePair<string, string>
{
    public List<T> stats = new List<T>();

    public bool Contains(string name)
    {
        foreach(T s in stats)
            if (name == s.Name)
                return true;
        return false;
    }

    public T Get(string name)
    {
        foreach (T s in stats)
            if (name == s.Name)
                return s;
        return default(T);
    }

    public void Add(T item)
    {
        stats.Add(item);
    }
}

public interface IKeyValuePair<TKey,TValue>
{
    TKey Name { get; set; }
    TValue Value { get; set; }
}

[System.Serializable]
public class JStat : IKeyValuePair<string,string>
{
    public string name;
    public string value;

    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    public string Value
    {
        get { return value; }
        set { this.value = value; }
    }
}
