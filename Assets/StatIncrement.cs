using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatIncrement : MonoBehaviour
{
    public bool onStart = true;
    [Space]
    public string statName = "stat";
    public int statIncrement = 1;

    void OnEnable()
    {
        if (onStart)
            Increment();
    }

    public void Increment()
    {
        Stats.j.Increment(statName, statIncrement);
    }
}
