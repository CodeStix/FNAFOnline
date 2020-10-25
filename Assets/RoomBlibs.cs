using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomBlibs : MonoBehaviour
{
    public GameObject pointer;
    public RoomScheme scheme;
    [Space]
    public float pointerMoveSpeed = 1f;
    [Space]
    public bool onlyShowPointer = false;

    private List<MapBlib> blibs = new List<MapBlib>();
    private static List<RoomBlibs> roomBlibs;
    private Vector3 pointerTarget;
    private RoomConnection last;

    void Awake()
    {
        pointerTarget = pointer.transform.localPosition;

        if (roomBlibs == null)
            roomBlibs = new List<RoomBlibs>();

        if (!roomBlibs.Contains(this))
            roomBlibs.Add(this);

        for(int i = 0; i < transform.childCount; i++)
        {
            MapBlib b = transform.GetChild(i).GetComponent<MapBlib>();
            if (b != null)
                blibs.Add(b);
        }
            
        foreach (MapBlib blib in blibs)
            blib.hidden = true;

        scheme.OnMove += Moved;
    }

    void OnEnable()
    {
        UpdateRoomPointer();
    }

    void Update()
    {
        pointer.transform.localPosition = Vector3.Lerp(pointer.transform.localPosition, pointerTarget, Time.deltaTime * pointerMoveSpeed);
    }

    public static void RefreshAllBlibs()
    {
        foreach (RoomBlibs b in roomBlibs)
            b.RefreshBlibs();
    }

    private void Moved(RoomConnection room)
    {
        try
        {
            RefreshAllBlibs();
        }
        catch
        {
            RefreshBlibs();
        }

        last = room;

        UpdateRoomPointer();
    }

    private void UpdateRoomPointer()
    {
        if (last != null)
            pointerTarget = GetBlibForRoomName(last.roomName).transform.localPosition;
    }

    public void RefreshBlibs()
    {
        foreach (MapBlib blib in blibs)
            blib.hidden = true;

        if (!scheme.enable)
            return;

        foreach (RoomConnection con in scheme.GetAvailableRoomsInRange())
        {
            MapBlib blib = GetBlibForRoomName(con.roomName);

            blib.hidden = onlyShowPointer;
            blib.disabled = false;

            if (!con.CanMoveHere())
                blib.disabled = true;
        }
    }

    private MapBlib GetBlibForRoomName(string name)
    {
        return blibs.Where((e) => e.gameObject.name == name).First();
    }

    public void OnlyShowPointer()
    {
        onlyShowPointer = true;
    }
}
