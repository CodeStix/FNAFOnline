using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FNAFMonsterLocation : MonoBehaviour
{
    public GameObject[] states;
    [Tooltip("Optional, the move button that controls this location")]
    public FNAFMoveButton moveButton;
    //[Tooltip("Optional, the camera that is looking at this location")]
    //public FNAFOffice1Camera lookingCamera;
    public bool isFar = true; // Will be used to know if should play far or close sound

    private int current = -1;
     
    public void SetState(int state)
    {
        if (state == current) return;

        if (current >= 0 && states.Length > 0)
            states[current].SetActive(false);

        if (state >= 0 && state < states.Length && states.Length > 0)
            states[state].SetActive(true);

        current = state;

        moveButton.SetIsHere(state >= 0);
    }

    public void SetMoveButton(bool isHere)
    {
        if (moveButton != null)
            moveButton.SetIsHere(isHere);
    }
}
