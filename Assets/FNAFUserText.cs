using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FNAFUserText : MonoBehaviour
{
    public Text text;
    public string format = "{0} <color=#aaeeaa>Night {1}</color>";

    void Start()
    {
        text.text = "";
    }

    void Update()
    {
        FNAFUser me = FNAFClient.Instance.GetUser();
        text.text = string.Format(format, me.name, me.night);
    }
}
