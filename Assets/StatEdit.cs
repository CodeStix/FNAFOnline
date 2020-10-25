using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatEdit : MonoBehaviour
{
    public bool onStart = true;
    [Space]
    public string statName = "stat";
    public StatEditFunction function;
    [Help("'statFunctionArgument' only used if function is set to Set, Multiply or Increment.")]
    public int statFunctionArgument = 0;
    [Help("'statFunctionMerge' only used if function is set to Merge or Copy.")]
    public string statFunctionMerge = "fromStat";

    void OnEnable()
    {
        if (onStart)
            EditStat();
    }

    public void EditStat()
    {
        if (function == StatEditFunction.Set)
        {
            Stats.j.Put(statName, statFunctionArgument);
        }
        else if (function == StatEditFunction.Increment)
        {
            Stats.j.Increment(statName, statFunctionArgument);
        }
        else if (function == StatEditFunction.Merge)
        {
            Stats.j.Increment(statName, Stats.j.GetInt(statFunctionMerge));
        }
        else if (function == StatEditFunction.Copy)
        {
            Stats.j.Put(statName, Stats.j.GetInt(statFunctionMerge));
        }
        else if (function == StatEditFunction.Reset)
        {
            Stats.j.Put(statName, 0);
        }
        else if (function == StatEditFunction.Multiply)
        {
            Stats.j.Put(statName, Stats.j.GetInt(statName) * statFunctionArgument);
        }
    }
}

[System.Serializable]
public enum StatEditFunction
{
    Set,
    Increment,
    Merge,
    Copy,
    Reset,
    Multiply
}
