using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FNAFScrollingImage : MonoBehaviour
{
    public float minX, maxX;
    public float speed;

    private bool goingLeft = false;

    private void Update()
    {
        if (goingLeft)
        {
            transform.localPosition -= new Vector3(speed, 0, 0) * Time.deltaTime;
            if (transform.localPosition.x <= minX)
            {
                goingLeft = false;
            }
        }   
        else
        {
            transform.localPosition += new Vector3(speed, 0, 0) * Time.deltaTime;
            if (transform.localPosition.x >= maxX)
            {
                goingLeft = true;
            }
        }
    }
}
