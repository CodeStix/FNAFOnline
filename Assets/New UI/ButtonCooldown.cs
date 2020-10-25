using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonCooldown : MonoBehaviour
{
    public float cooldown = 10f;
    public ButtonCooldownMode mode = ButtonCooldownMode.EnableDisable;
    public bool coolOnStart = true;

    private Button button;
    private float time = 0f;

    void Start()
    {
        button = GetComponent<Button>();

        if (coolOnStart)
            ButtonGotClicked();

        button.onClick.AddListener(() => 
        {
            ButtonGotClicked();
        });
    }

    private void ButtonGotClicked()
    {
        if (mode == ButtonCooldownMode.EnableDisable)
        {
            button.enabled = false;
        }
        else if (mode == ButtonCooldownMode.InteractableNotInteractable)
        {
            button.interactable = false;
        }

        time = Time.time + cooldown;
    }

    void Update()
    {
        if (time < Time.time)
        {
            if (mode == ButtonCooldownMode.EnableDisable)
            {
                button.enabled = true;
            }
            else if (mode == ButtonCooldownMode.InteractableNotInteractable)
            {
                button.interactable = true;
            }
        }
    }
	
}

public enum ButtonCooldownMode
{
    EnableDisable,
    InteractableNotInteractable
}