using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DangerBox : MonoBehaviour
{
    public string showAnimation;
    public string hideAnimation;
    [Space]
    public Text mainMessageText;
    public Text subMessageText;

    public bool IsShown
    {
        get
        {
            return shown;
        }
    }

    private Animation m;
    private bool shown = false;

    void Start()
    {
        m = GetComponent<Animation>();
    }

    public void Show(string mainMessage)
        => Show(mainMessage, "");

    public void Show(string mainMessage, string subMessage)
    {
        mainMessageText.text = mainMessage;
        subMessageText.text = subMessage;

        m.Play(showAnimation);

        shown = true;
    }

    public void Hide()
    {
        m.Play(hideAnimation);

        shown = false;
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
