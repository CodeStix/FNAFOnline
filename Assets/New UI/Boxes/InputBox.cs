using Stx.Net.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Animation))]
public class InputBox : MonoBehaviour
{
    public string showAnimation;
    public string hideAnimation;
    public string invalidInputAnimation;
    [Space]
    public Text mainInstructionText;
    public Text subInstructionText;
    public InputField input;

    private Animation m;
    private Func<string, bool> submit;
    private bool shown = false;

    void Start()
    {
        m = GetComponent<Animation>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && shown)
        {
            Cancel();
        }
        else if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && shown)
        {
            Ok();
        }
    }

    public void AskForInput(string mainInstruction, string subInstruction, Func<string, bool> submitResult)
    {
        submit = submitResult;

        input.text = string.Empty;
        input.Select();
        input.ActivateInputField();
        
        mainInstructionText.text = mainInstruction;
        subInstructionText.text = subInstruction;

        m.Play(showAnimation);
        shown = true;
    }

    public void Ok()
    {
        if (submit.Invoke(input.text))
        {
            m.Play(hideAnimation);
            shown = false;
        }
        else
        {
            if (!string.IsNullOrEmpty(invalidInputAnimation))
                m.Play(invalidInputAnimation);
        }
    }

    public void Cancel()
    {
        if (submit.Invoke(null))
        {
            m.Play(hideAnimation);
            shown = false;
        }
        else
        {
            if (!string.IsNullOrEmpty(invalidInputAnimation))
                m.Play(invalidInputAnimation);

            input.text = "";
        }
    }
}
