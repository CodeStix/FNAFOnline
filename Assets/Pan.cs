using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    public float maxX = 1f;
    public float minX = -1f;
    public float panSpeed = 0.01f;

    private bool direction = true;

    void Update()
    {
        Vector3 pos = transform.localPosition;

        if (direction)
        {
            pos.x += panSpeed;
        }
        else
        {
            pos.x -= panSpeed;
        }

        if (pos.x > maxX)
        {
            pos.x = maxX;
            direction = false;
        }

        if (pos.x < minX)
        {
            pos.x = minX;
            direction = true;
        }

        transform.localPosition = pos;
    }
    
}
