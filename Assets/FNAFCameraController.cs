using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FNAFCameraController : MonoBehaviour
{
    public Camera mainCamera;
    public float speed = 1f;
    public float treshold = 100f;

    private float minX, maxX;

    void Start()
    {
        // 1600 is the size of the office in pixels
        Debug.Log("screen width = " + Screen.width);
        float x = mainCamera.ScreenToWorldPoint(new Vector2(Mathf.Max(1600 - Screen.width, 0f), 0f)).x;
        Debug.Log("x = " + x);
        minX = x;
        maxX = -x;
    }

    void Update()
    {
        float cameraWidth = Screen.width;

        Vector3 mouse = Input.mousePosition;

        if (mouse.x < treshold)
        {
            Vector3 pos = transform.localPosition;

            pos.x -= Time.deltaTime * speed;
            if (pos.x < minX) 
                pos.x = minX;

            transform.localPosition = pos;

            mainCamera.
        }

        if (mouse.x > Screen.width - treshold)
        {
            Vector3 pos = transform.localPosition;

            pos.x += Time.deltaTime * speed;
            if (pos.x > maxX)
                pos.x = maxX;

            transform.localPosition = pos;
        }

    }
}
