using FNAFOnline.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MoveTimer : MonoBehaviour
{
    public Night night;
    [Space]
    public Text moveTimerText;
    public MoveTimings timings;
    [Space]
    public UnityEvent onCanMove;
    public UnityEvent onMustWait;

    public delegate void OnCanMoveDelegate();
    public event OnCanMoveDelegate OnCanMove;

    private float time;

    void Start()
    {
        //Refresh(timings.StartWait);
    }

    public void Refresh()
    {
        Refresh(0f);
    }

    public void Refresh(float extraTime = 0f)
    {
        time = extraTime + timings.SecondsPerMove + Random.Range(-timings.MaxRandomness, timings.MaxRandomness) - (night.GetNightProgress() * timings.DecreaseOverNight);
        MapBlib.globalDisable = true;

        onMustWait?.Invoke();
    }

    void Update()
    {
        time -= Time.deltaTime;

        if (time <= 0f && time > -1f)
        {
            moveTimerText.text = "Move!";

            MapBlib.globalDisable = false;

            onCanMove?.Invoke();

            if (OnCanMove != null)
                OnCanMove.Invoke();

            time = -1f;
        }
        else if (time > 0f)
        {
            moveTimerText.text = Mathf.CeilToInt(time).ToString();
        }
    }

}

/*[System.Serializable]
public class MoveTimings
{
    public float secondsPerMove = 15f;
    public float maxRandomness = 7.5f;
    [Range(0f, 20f)]
    public float difficulty = 5f;
    public float startWait = 8f;
}*/