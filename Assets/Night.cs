using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class Night : MonoBehaviour
{
    public bool startNightOnStart = false;
    [Header("Power")]
    public bool usePower = true;
    public States powerUsage;
    public float powerDrainSpeed = 0.2f;
    public float startingPower = 100;
    public Text powerText;
    public UnityEvent onPowerZero;
    [Header("Time")]
    public float nightSeconds = 180;
    public float secondsPerHour = 30;
    public string[] hours = new string[] { "12PM", "1AM", "2AM", "3AM", "4AM", "5AM", "6AM" };
    public Text hourText;
    public Text timeText;
    public UnityEvent onNightDone;

    private float power;
    private int roundedPower;
    private float time;
    private int hour = 0;
    private bool isStarted = false;

    void Start()
    {
        if (startNightOnStart)
            StartNight();
    }

    void Update()
    {
        if (!isStarted)
            return;

        if (usePower)
        {
            power -= ((powerUsage.GetCurrentState() * 2) + 1) * powerDrainSpeed * Time.deltaTime;

            int r = Mathf.FloorToInt(power);

            if (roundedPower != r)
            {
                roundedPower = r;

                UpdatePowerText();

                if (roundedPower == 0)
                    onPowerZero.Invoke();
            }
        }

        time -= Time.deltaTime;

        int h = Mathf.FloorToInt((nightSeconds - time) / secondsPerHour);

        if (h != hour)
        {
            hour = h;

            UpdateHourText();

            if (time <= 0f)
                EndNight();
        }

        UpdateTimeText();
    }

    public void StartNight()
    {
        if (usePower)
        {
            power = startingPower;
            roundedPower = Mathf.FloorToInt(power);

            UpdatePowerText();
        }

        time = nightSeconds;

        UpdateHourText();
        UpdateTimeText();

        isStarted = true;

        powerUsage.JumpFirst();
    }

    private void UpdatePowerText()
    {
        if (usePower)
            powerText.text = roundedPower + "%";
    }

    private void UpdateHourText()
    {
        hourText.text = hours[hour];
    }

    private void UpdateTimeText()
    {
        timeText.text = Math.Round(time,1).ToString();
    }

    public int GetHour()
    {
        return hour;
    }

    public void PowerDown()
    {
        power = 0.9f;
    }

    public void SetPower(int value)
    {
        this.power = value;
        this.roundedPower = value;

        UpdatePowerText();
    }

    public void AddPower(int value)
    {
        this.power += value;
        this.roundedPower += value;

        UpdatePowerText();
    }

    public float GetNightProgress()
    {
        return (nightSeconds - time) / nightSeconds;
    }

    public void EndNight()
    {
        if (onNightDone != null)
            onNightDone.Invoke();

        isStarted = false;
    }
}
