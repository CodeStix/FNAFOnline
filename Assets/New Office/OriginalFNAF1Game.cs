using FNAFOnline.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Stx.Net;
using Stx.Net.Unity;
using Stx.Utilities;

public class OriginalFNAF1Game : MonoBehaviour
{
    public Text powerLeftText;
    [Space]
    public Office office;
    public Monitor monitor;

    public GameSetup gameSetup;

    void OnEnable()
    {
        StxUnityClient.C.DataReceiver.AddHandler(new DataHandler<int>("FNAF1Power", OnFNAFPowerChange));
        StxUnityClient.C.DataReceiver.AddHandler(new ObjectHandler("FNAF1Office", OnFNAFOfficeChange));
        StxUnityClient.C.DataReceiver.AddHandler(new ObjectHandler("FNAF1Move", OnFNAFMove));

        // If the player is guard, disable the boot sequence.
        //if (gameSetup.GuardClientID == StxUnityClient.C.NetworkID)
        //{
        //    monitor.startupTime = 0f;
        //}
    }

    void OnDisable()
    {
        StxUnityClient.C.DataReceiver.RemoveHandler("FNAF1Power");
        StxUnityClient.C.DataReceiver.RemoveHandler("FNAF1Office");
        StxUnityClient.C.DataReceiver.RemoveHandler("FNAF1Move");
    }

    private void OnFNAFPowerChange(int newPower, IDataHolder data)
    {
        powerLeftText.text = newPower.ToString();
    }

    private void OnFNAFOfficeChange(object value, IDataHolder data)
    {
        /*      ["FNAF1Office"] = true,
                ["CurrentCamera"] = currentCamera,
                ["DoorR"] = isRightDoorClosed,
                ["DoorL"] = isLeftDoorClosed,
                ["LightR"] = isRightLightOn,
                ["LightL"] = isLeftLightOn*/

        if (data.Data.Requires<bool>("DoorR"))
            office.SetRightDoor((bool)data.Data["DoorR"]);
        if (data.Data.Requires<bool>("DoorL"))
            office.SetLeftDoor((bool)data.Data["DoorL"]);
        if (data.Data.Requires<bool>("LightR"))
            office.SetRightLight((bool)data.Data["LightR"]);
        if (data.Data.Requires<bool>("LightL"))
            office.SetLeftLight((bool)data.Data["LightL"]);
    }

    private void OnFNAFMove(object value, IDataHolder data)
    {

    }

    public void SendToggleRightDoor()
    {
        StxUnityClient.F.RequestAsync("FNAF1Office", new Hashtable()
        {
            ["DoorR"] = !office.GetRightDoor()
        });
    }

    public void SendToggleLeftDoor()
    {
        StxUnityClient.F.RequestAsync("FNAF1Office", new Hashtable()
        {
            ["DoorL"] = !office.GetLeftDoor()
        });
    }

    public void SendToggleRightLight()
    {
        StxUnityClient.F.RequestAsync("FNAF1Office", new Hashtable()
        {
            ["LightR"] = !office.GetRightLight()
        });
    }

    public void SendToggleLeftLight()
    {
        StxUnityClient.F.RequestAsync("FNAF1Office", new Hashtable()
        {
            ["LightL"] = !office.GetLeftLight()
        });
    }
}
