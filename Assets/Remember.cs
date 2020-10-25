using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Remember : MonoBehaviour
{
    public string saveFile = "r1";

    public GameObject[] toEnableFirstTime;
    public GameObject[] toEnable;

    void Awake()
    {
        if (!File.Exists(saveFile))
        {
            File.WriteAllText(saveFile, 1.ToString());

            foreach (GameObject obj in toEnableFirstTime)
                obj.SetActive(true);
        }
        else
        {
            foreach (GameObject obj in toEnable)
                obj.SetActive(true);
        }
    }
}
