using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FNAFCameraController : MonoBehaviour
{
    public float minX, maxX;
    public Camera mainCamera;
    public float speed = 1f;
    public float treshold = 100f;

    //private float cameraWidth;

    void Start()
    {
        //cameraWidth = mainCamera.orthographicSize * ((float)Screen.width / Screen.height);
    }

    void Update()
    {
        Vector3 mouse = Input.mousePosition;

        float cameraWidth = mainCamera.orthographicSize * ((float)Screen.width / Screen.height);

        if (cameraWidth * 2 >= maxX - minX)
            return;

        if (mouse.x < treshold)
        {
            Vector3 pos = transform.localPosition;

            pos.x -= Time.deltaTime * speed;
            if (pos.x < minX + cameraWidth) 
                pos.x = minX + cameraWidth;

            transform.localPosition = pos;

        }

        if (mouse.x > Screen.width - treshold)
        {
            Vector3 pos = transform.localPosition;

            pos.x += Time.deltaTime * speed;
            if (pos.x > maxX - cameraWidth)
                pos.x = maxX - cameraWidth;

            transform.localPosition = pos;
        }

    }
}
