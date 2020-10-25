using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorChecker : MonoBehaviour
{
    public States door;
    public RoomConnection doorConnection;
    public UnityEvent onPresent;
    public float minCheckTime = 4f;
    public float maxCheckTime = 14f;

    private float time = 0;

    void Start()
    {
        time = 40f;
    }

    void Update()
    {
        time -= Time.deltaTime;

        if (time <= 0)
        {
            if (door.GetCurrentState() == 1 && doorConnection.isHere)
                onPresent.Invoke();//doorConnection.GetScheme().ForceMove(doorConnection.GetScheme().startingLocation);

            time = Random.Range(minCheckTime, maxCheckTime);
        }
    }
}
