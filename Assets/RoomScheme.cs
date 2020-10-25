using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomScheme : MonoBehaviour
{
    public bool enable = true;
    public RoomConnection[] startingLocations;
    public int moverID = 0;
    public UnityEvent onMove;
    [Space]
    [Range(0f,1f)]
    public float randomMovePreferChance = 0.5f;

    public delegate void OnMoveDelegate(RoomConnection room);
    public event OnMoveDelegate OnMove;

    public delegate void OnLeaveDelegate(RoomConnection room);
    public event OnLeaveDelegate OnLeave;

    public static Dictionary<int, RoomConnection> current;

    //private RoomConnection[] allRooms;

    void Start()
    {
        if (current == null)
            current = new Dictionary<int, RoomConnection>();

        //allRooms = transform.GetComponentsInChildren<RoomConnection>();

        RoomConnection start = startingLocations[0];

        if (!current.ContainsKey(moverID))
            current.Add(moverID, start);
        else
            current[moverID] = start;

        current[moverID].MoveHere();

        if (OnMove != null)
            OnMove(start);
    }

    public RoomConnection GetRandomStartingRoom()
    {
        return startingLocations[Random.Range(0, startingLocations.Length)];
    }

    public RoomConnection[] GetAvailableRoomsInRange()
    {
        return current[moverID].connections;
    }

    public RoomConnection GetCurrentRoom()
    {
        return current[moverID];
    }

    public void ForceMoveNamed(string roomName)
    {
        Transform room = transform.Find(roomName);
        RoomConnection connection = room.GetComponent<RoomConnection>();
        ForceMove(connection);
    }

    public void MoveNamed(string roomName)
    {
        Transform room = transform.Find(roomName);
        RoomConnection connection = room.GetComponent<RoomConnection>();
        Move(connection);
    }

    public bool Move(RoomConnection room)
    {
        if (!enable)
            return false;

        foreach(RoomConnection r in GetAvailableRoomsInRange())
        {
            if (room == r)
            {
                if (!room.CanMoveHere())
                    return false;
                ForceMove(room);
                return true;
            }
        }
        return false;
    }

    public void ForceMove(RoomConnection room)
    {
        if (!enable)
            return;
        if (OnLeave != null)
            OnLeave.Invoke(current[moverID]);
        current[moverID].Leave();
        room.MoveHere();
        current[moverID] = room;
        if (OnMove != null)
            OnMove.Invoke(room);
        if (onMove != null)
            onMove.Invoke();
    }

    public void ForceMoveBack()
    {
        ForceMove(GetRandomStartingRoom());
    }

    public void MoveRandomly(string[] preferRoomNames = null)
    {
        if (!enable)
            return;

        RoomConnection[] connections = GetAvailableRoomsInRange();

        RoomConnection connection = null;

        if (preferRoomNames != null)
        {
            foreach (RoomConnection c in connections)
            {
                foreach(string p in preferRoomNames)
                {
                    if (c.roomName == p && Random.Range(0f, 1f) <= randomMovePreferChance)
                    {
                        connection = c;
                        break;
                    }
                }
            }

            if (connections.Length == 0)
                return;

            if (connection == null)
                connection = connections[Random.Range(0, connections.Length)];
        }
        else
        {
            connection = connections[Random.Range(0, connections.Length)];
        }
          
        bool succes;
        int tries = 0;
        do
        {
            tries++;
            succes = Move(connection);
        }
        while (!succes && tries < 50);
    }
	
}
