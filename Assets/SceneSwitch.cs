using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public string sceneName;
    public float delay = 0f;
    public bool onStart = false;

    void OnEnable()
    {
        if (onStart)
            Switch();
    }

    public void Switch()
    {
        Invoke(nameof(SwitchLater), delay);
    }

    private void SwitchLater()
    {
        LoadingScreen.LoadScene(sceneName);
    }
}
