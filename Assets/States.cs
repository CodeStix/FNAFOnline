using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class States : MonoBehaviour
{
    public int startingState = 0;
    public bool randomStartingState = false;
    public bool randomizeOnEnable = false;

    public delegate void OnChangeDelegate(int newState);
    public event OnChangeDelegate OnChange;

    private GameObject[] states = new GameObject[] { };
    private int current = 0;


    void OnEnable()
    {
        if (randomizeOnEnable)
            JumpRandom();
    }

    private void Awake()
    {
        states = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            states[i] = transform.GetChild(i).gameObject;

        if (randomStartingState)
            current = Random.Range(0, states.Length);
        else
            current = startingState;
    }

    void Start()
    {
        Debug.Log(gameObject.name + " has starting state " + current);

        UpdateStates();
    }

    public void Previous()
    {
        if (--current < 0)
            JumpLast();
        else
            UpdateStates();

        Debug.Log(gameObject.name + " jumps to previous state " + current);
    }

    public void Next()
    {
        if (++current >= states.Length)
            JumpFirst();
        else
            UpdateStates();

        Debug.Log(gameObject.name + " jumps to next state " + current);
    }

    public void JumpFirst()
    {
        current = 0;

        UpdateStates();
    }

    public void JumpLast()
    {
        current = states.Length - 1;

        Debug.Log(gameObject.name + " jumps to last state " + current);

        UpdateStates();
    }

    public void ToggleFirstSecond()
    {
        if (current == 0)
            current = 1;
        else if (current == 1)
            current = 0;

        Debug.Log(gameObject.name + " jumps to first/second state " + current);

        UpdateStates();
    }

    public void JumpRandom()
    {
        current = Random.Range(0, states.Length);

        Debug.Log(gameObject.name + " jumps to random state " + current);

        UpdateStates();
    }

    public void Jump(int to)
    {
        Debug.Log(gameObject.name + " jumps to state " + to);

        current = Mathf.Clamp(to, 0, states.Length - 1);

        UpdateStates();
    }

    public int GetCurrentState()
    {
        return current;
    }
        
    public GameObject GetCurrentStateObject()
    {
        return states[current];
    }

    public string GetCurrentStateName()
    {
        return GetCurrentStateObject().name;
    }

    private void UpdateStates()
    {
        for(int i = 0; i < states.Length; i++)
        {
            GameObject obj = states[i];
            obj.SetActive(current == i);
        }

        if (OnChange != null)
            OnChange.Invoke(current);
    }


}
