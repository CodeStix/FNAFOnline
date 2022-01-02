using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FNAFMoveIndicator : MonoBehaviour
{
    public List<FNAFMoveButton> buttons;
    public float speed;

    void Update()
    {
        int index = buttons.FindIndex((e) => e.IsHere);
        if (index < 0)
        {
            return;    
        }

        FNAFMoveButton target = buttons[index];

        transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime * speed);
    }
}
