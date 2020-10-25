using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Text progressText;
    public Slider progressSlider;
    public Image[] imagesToFade;
    public float sliderLerpSpeed = 1f;
    public float backgroundLerpSpeed = 1f;
    public string progressFormat = "{1} {0}%";
    [Space]
    public bool main = false;

    private float progressTarget = 0f;
    private float alphaTarget = 0f;
    private string status = "Loading... ";
    private float previousAlpha = 0f;
    private bool enableMarquee = false;

    private static LoadingScreen mainInstance;

    void Awake()
    {
        if (main)
            mainInstance = this;
    }

    void Update()
    {
        SetBackAlpha();

        if (enableMarquee)
        {
            if (progressSlider.value <= 0.26f)
                progressTarget = 0.75f;
            if (progressSlider.value >= 0.74f)
                progressTarget = 0.25f;
        }

        progressSlider.value = Mathf.Lerp(progressSlider.value, progressTarget, sliderLerpSpeed * Time.deltaTime);
        progressText.text = string.Format(progressFormat, Mathf.CeilToInt(progressSlider.value * 100.0f), status);
    }
    
    public void Progress(float p)
    {
        progressTarget = p;

        if (p == 0f)
        {
            progressSlider.value = 0f;
        }

        enableMarquee = false;

        if (p >= 1f)
        {
            alphaTarget = 0f;
        }
        else
        {
            alphaTarget = 1f;
        }
    }

    public void Done()
    {
        Progress(1f);
    }

    public void Marquee()
    {
        alphaTarget = 1f;
        enableMarquee = true;
    }

    public void Status(string str)
    {
        status = str;
    }

    public static void LoadScene(string scene)
    {
        mainInstance.LoadSceneAsync(scene);
    }

    public void LoadSceneAsync(string scene)
    {
        Progress(0f);
        Status("Loading " + scene + "...");

        StartCoroutine(ILoadSceneAsync(scene));
    }

    private IEnumerator ILoadSceneAsync(string sceneName)
    {
        Progress(0f);

        yield return new WaitForSeconds(0.25f);

        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        Status("Unloading...");

        while (!unloadOperation.isDone)
        {
            Progress(unloadOperation.progress * 0.3f);

            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadOperation.allowSceneActivation = false;

        Status("Loading...");

        while (!loadOperation.isDone)
        {
            Progress(0.3f + loadOperation.progress * 0.6f);

            if (loadOperation.progress >= 0.9f)
            {
                loadOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        yield return new WaitForSeconds(0.35f);

        Progress(1f);
    }

    /*private IEnumerator ILoadSceneAsync(string scene)
    {
        Progress(0f);

        yield return new WaitForSeconds(0.3f);

        AsyncOperation ao = SceneManager.LoadSceneAsync(scene);
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            Progress(ao.progress);

            if (ao.progress >= 0.9f)
            {
                ao.allowSceneActivation = true;
            }

            yield return null;
        }

        Progress(1f);
    }*/

    private void SetBackAlpha()
    {
        float a = Mathf.Lerp(previousAlpha, alphaTarget, backgroundLerpSpeed * Time.deltaTime);
        previousAlpha = a;

        Color c = progressText.color;
        c.a = a;
        progressText.color = c;

        foreach(Image m in imagesToFade)
        {
            if (a < 0.1f)
            {
                m.enabled = false;
                continue;
            }
            else
            {
                m.enabled = true;
                c = m.color;
                c.a = a;
                m.color = c;
            }
        }

        progressSlider.enabled = a > 0.15f;
    }

}
