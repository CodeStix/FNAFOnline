using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FNAFMonsterLocation : MonoBehaviour
{
    public GameObject[] states;
    [Tooltip("Optional, the move button that controls this location")]
    public FNAFMoveButton moveButton;
    [Tooltip("Optional, the camera that is looking at this location")]
    public FNAFOffice1Camera lookingCamera; 

    private int current = -1;
     
    private void Start()
    {
        //foreach (GameObject obj in states)
        //{
        //    obj.SetActive(false);
        //}
    }

    public void SetState(int state)
    {
        if (state == current) return;

        if (current >= 0 && states.Length > 0)
            states[current].SetActive(false);

        if (state >= 0 && state < states.Length && states.Length > 0)
            states[state].SetActive(true);

        if (moveButton != null)
            moveButton.SetIsHere(state >= 0);

        current = state;
    }
}
