using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public bool enable = true;
    public GameObject toFollow;
    public float lerpSpeed;

    private Vector3 startingPosition;

    void Start ()
    {
        startingPosition = transform.position;
    }
	
	void Update ()
    {
        if (!enable)
        {
            transform.position = startingPosition;
            return;
        }

        Vector3 pos = transform.position;

        Vector3 gotoPos = toFollow.transform.position;

        pos.x = Mathf.Lerp(pos.x, gotoPos.x, lerpSpeed * Time.deltaTime);
        pos.y = Mathf.Lerp(pos.y, gotoPos.y, lerpSpeed * Time.deltaTime);

        transform.position = pos;
	}

    public void Disable()
    {
        enable = false;
    }
}
