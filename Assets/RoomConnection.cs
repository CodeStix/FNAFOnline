using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomConnection : MonoBehaviour
{
    public RoomConnection[] connections;
    public bool isHere = false;
    public int maxMovesHereAtOnce = 1;
    public GameObject obstacle;
    public UnityEvent onMove;
    public UnityEvent onLeave;

    [HideInInspector]
    public string roomName;

    private RoomScheme scheme;

    private static Dictionary<string, int> moved;

    void Awake()
    {
        roomName = gameObject.name;
        scheme = transform.parent.GetComponent<RoomScheme>();

        if (moved == null)
            moved = new Dictionary<string, int>();

        if (!moved.ContainsKey(roomName))
            moved.Add(roomName, 0);
        else
            moved[roomName] = 0;
    }

    public void MoveHere()
    {
        moved[roomName]++;
        isHere = true;
        onMove.Invoke();
        // Debug.Log("Moved(" + transform.parent.name + ") to " + gameObject.name);
    }

    public void Leave()
    {
        moved[roomName]--;
        isHere = false;
        onLeave.Invoke();
        // Debug.Log("Leaving(" + transform.parent.name + ") from " + gameObject.name);
    }

    public bool CanMoveHere()
    {
        bool o = true;
        if (obstacle != null)
            o = !obstacle.activeSelf;

        return !(moved[roomName] >= maxMovesHereAtOnce) && o;
    }

    public RoomScheme GetScheme()
    {
        return scheme;
    }
}