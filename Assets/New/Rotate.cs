using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float step = 1f;

    void Start()
    {
        
    }

    void Update()
    {
        SetZRotation(step * Time.time);
    }

	public void SetZRotation(float z)
    {
        Vector3 r = transform.rotation.eulerAngles;

        r.z = z;

        transform.rotation = Quaternion.Euler(r);
    }
}
