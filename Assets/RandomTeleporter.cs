using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTeleporter : MonoBehaviour
{
    public Vector2 max;
    public Vector2 min;
    [Space]
    public bool onStart = false;

    void OnEnable()
    {
        if (onStart)
            Teleport();
    }

    public void Teleport()
    {
        Vector3 v = transform.position;

        v.x = Random.Range(min.x, max.x);
        v.y = Random.Range(min.y, max.y);

        transform.position = v;
    }
}
