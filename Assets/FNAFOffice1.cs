using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FNAFOffice1 : MonoBehaviour
{
    public FNAFOfficeMonitor monitor;

    private bool monitorOpen = false;

    public void ToggleMonitor()
    {
        monitorOpen = !monitorOpen;
        if (monitorOpen)
            monitor.Up();
        else
            monitor.Down();
    }
}
