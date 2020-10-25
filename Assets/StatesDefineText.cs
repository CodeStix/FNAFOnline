using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class StatesDefineText : MonoBehaviour {

    public States states;
    public string[] stateIndexMeaning;

    private Text text;

	void Start ()
    {
        text = GetComponent<Text>();

        states.OnChange += States_OnChange;
	}

    private void States_OnChange(int newState)
    {
        if (stateIndexMeaning.Length > newState)
        {
            text.text = stateIndexMeaning[newState];
        }
        else
        {
            Debug.LogWarning("Undefined state meaning in " + gameObject.name);
        }
    }
}
