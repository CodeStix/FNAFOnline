using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public float maxX = 1f;
    public float minX = -1f;
    public int screenHoverDistance = 100;
    public float lerpSpeed = 1f;

	void Start ()
    {
		
	}
	
	void FixedUpdate ()
    {
        int x = Mathf.RoundToInt(Input.mousePosition.x);
        int width = Screen.currentResolution.width;

        Vector3 pos = transform.position;

        if (x <= screenHoverDistance)
        {
            pos.x = Mathf.Clamp(Mathf.Lerp(pos.x, minX, Time.deltaTime * lerpSpeed), minX, maxX);
        }

        if (x >= width - screenHoverDistance)
        {
            pos.x = Mathf.Clamp(Mathf.Lerp(pos.x, maxX, Time.deltaTime * lerpSpeed), minX, maxX);
        }

        transform.position = pos;
	}
}
