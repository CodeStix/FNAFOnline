using FNAFOnline.Shared;
using Stx.Net;
using Stx.Net.Unity;
using Stx.Net.VoiceBytes.Unity;
using Stx.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class NetworkFunctions : MonoBehaviour
{
    public RoomScheme bonnieRoomScheme;
    public RoomScheme chicaRoomScheme;
    public RoomScheme freddyRoomScheme;
    public RoomScheme foxyRoomScheme;
    public RoomScheme goldenFreddyRoomScheme;
    //[Space]
    //public GameObject[] toDisableIfSecurity;
    //public GameObject[] toDisableIfNotSecurity;
    [Space]
    public States leftDoor;
    public States rightDoor;
    [Space]
    public Cameras cameras;
    public GameObject cameraButtons;
    //[Space]
    //public States[] doorsLightsButtons;
    [Space]
    public States office;
    [Space]
    public Night night;
    public UnityEvent onPowerZeroNotSecurity;
    [Space]
    public States securityGuardBlibs;
    [Space]
    public States phoneGuy;
    public GameObject phone;
    [Space]
    public UnityEvent onDistract;
    public UnityEvent onMayStart;
    public UnityEvent onExit;
    [Space]
    public LoadingScreen waitingPlayersScreen;
    [Space]
    public UnityEvent onIncomingCall;
    public UnityEvent onCallAccepted;
    public UnityEvent onStopCall;

    void OnEnable()
    {
        waitingPlayersScreen.Status("Waiting for other players...");
        waitingPlayersScreen.Progress(0.6f);

        StxUnityClient.C.DataReceiver.AddHandler(new ObjectHandler("FNAFEntityMoved", (e, p) =>
        {
            Debug.Log("Received move.");

            if (p.Data.Requires<string, string, bool>("Entity", "To", "Back"))
            {
                string entity = (string)p.Data["Entity"];
                string to = (string)p.Data["To"];
                bool back = (bool)p.Data["Back"];

                Debug.Log("Setting move: " + entity + " to " + to);
                SetMove(entity, to, back);
            }
        }));

        StxUnityClient.C.DataReceiver.AddHandler(new ObjectHandler("FNAFOfficeChanged", (e, p) =>
        {
            if (p.Data.Requires<string>("Door"))
                SetDoor((string)p.Data["Door"]);
            else if (p.Data.Requires<string>("Light"))
                SetLight((string)p.Data["Light"]);
            else if (p.Data.Requires<string>("LookingMapBlib"))
                SetLookingMapBlib((string)p.Data["LookingMapBlib"]);
            else if (p.Data.ContainsKey("ZeroPower"))
                SetZeroPower();
            else if (p.Data.Requires<string>("SendBackGoldenFreddy"))
                SetMove("GoldenFreddy", (string)p.Data["SendBackGoldenFreddy"], true);
        }));

        StxUnityClient.C.DataReceiver.AddHandler(new ObjectHandler("FNAFHacked", (e, p) =>
        {
            if (p.Data.Requires<int>("AddPower"))
                SetAddPower((int)p.Data["AddPower"]);
            if (p.Data.Contains("ItsMeDistraction"))
                SetItsMeDistraction();
            if (p.Data.Requires<bool>("Phone"))
                SetPhone((bool)p.Data["Phone"]);
        }));

        StxUnityClient.C.DataReceiver.AddHandler(new ObjectHandler("FNAFEveryoneLoaded", (e, p) =>
        {
            waitingPlayersScreen.Done();

            if (onMayStart != null)
                onMayStart.Invoke();
        }));
    }

    void OnDisable()
    {
        StxUnityClient.C.DataReceiver.RemoveHandler("FNAFEntityMoved");
        StxUnityClient.C.DataReceiver.RemoveHandler("FNAFOfficeChanged");
        StxUnityClient.C.DataReceiver.RemoveHandler("FNAFHacked");
        StxUnityClient.C.DataReceiver.RemoveHandler("FNAFGameCourse");
    }

    public void SendLeft()
    {
        SendEnd(new GameEndInfo(GameEndCause.SomeoneLeft, StxUnityClient.C.NetworkID));
    }

    public void SendEndOfNight()
    {
        SendEnd(new GameEndInfo(GameEndCause.NightOver, StxUnityClient.C.NetworkID));
    }

    public void SendDidDieByBonnie()
    {
        SendEnd(new GameEndInfo(GameEndCause.KilledByBonnie, StxUnityClient.C.NetworkID));
    }

    public void SendDidDieByChica()
    {
        SendEnd(new GameEndInfo(GameEndCause.KilledByChica, StxUnityClient.C.NetworkID));
    }

    public void SendDidDieByFreddy()
    {
        SendEnd(new GameEndInfo(GameEndCause.KilledByFreddy, StxUnityClient.C.NetworkID));
    }

    public void SendDidDieByFoxy()
    {
        SendEnd(new GameEndInfo(GameEndCause.KilledByFoxy, StxUnityClient.C.NetworkID));
    }

    public void SendDidDieByGoldenFreddy()
    {
        SendEnd(new GameEndInfo(GameEndCause.KilledByGoldenFreddy, StxUnityClient.C.NetworkID));
    }

    private void SendMove(string entity, string room, bool back)
    {
        Hashtable data = new Hashtable();
        data.Add("Entity", entity);
        data.Add("To", room);
        data.Add("Back", back);

        StxUnityClient.F.RequestAsync("FNAFMoveEntity", data);
    }

    private void SendHack(string hackName, object value)
    {
        Hashtable data = new Hashtable();
        data.Add(hackName, value);

        StxUnityClient.F.RequestAsync("FNAFHack", data);
    }

    private void SendOfficeChange(string changedName, object value)
    {
        Hashtable data = new Hashtable();
        data.Add(changedName, value);

        StxUnityClient.F.RequestAsync("FNAFOfficeChanged", data);
    }

    private void SendGameCourse(string changedName)
    {
        Hashtable data = new Hashtable();
        data.Add(changedName, true);

        StxUnityClient.F.RequestAsync("FNAFGameCourse", data);
    }

    private void SendEnd(GameEndInfo endInfo)
    {
        Hashtable data = new Hashtable();
        data.Add("EndGame", endInfo);

        StxUnityClient.F.RequestAsync("FNAFGameCourse", data);
    }

    private void SetMove(string entity, string to, bool back)
    {
        switch (entity)
        {
            case "Bonnie":
                SetMoveBonnie(to);
                if (back)
                    SoundEffects.Play("KnockLeft");
                break;
            case "Chica":
                SetMoveChica(to);
                if (back)
                    SoundEffects.Play("KnockRight");
                break;
            case "Freddy":
                SetMoveFreddy(to);
                if (back)
                    SoundEffects.Play("KnockRight");
                break;
            case "Foxy":
                SetMoveFoxy(to);
                if (back)
                    SoundEffects.Play("KnockLeft");
                break;
            case "GoldenFreddy":
                SetMoveGoldenFreddy(to);
                if (back)
                {
                    SoundEffects.Play("KnockLeft");
                    SoundEffects.Play("KnockRight");
                }
                break;
        }
    }

    public void SendMoveBonnie(string room)
    {
        SendMove("Bonnie", room, false);
    }

    public void SendMoveChica(string room)
    {
        SendMove("Chica", room, false);
    }

    public void SendMoveFreddy(string room)
    {
        SendMove("Freddy", room, false);
    }

    public void SendMoveFoxy(string room)
    {
        SendMove("Foxy", room, false);
    }

    public void SendMoveGoldenFreddy(string room)
    {
        SendMove("GoldenFreddy", room, false);
    }

    public void SetMoveBonnie(string room)
    {
        bonnieRoomScheme.ForceMoveNamed(room);
    }

    public void SetMoveChica(string room)
    {
        chicaRoomScheme.ForceMoveNamed(room);
    }

    public void SetMoveFreddy(string room)
    {
        freddyRoomScheme.ForceMoveNamed(room);
    }

    public void SetMoveFoxy(string room)
    {
        foxyRoomScheme.ForceMoveNamed(room);
    }

    public void SetMoveGoldenFreddy(string room)
    {
        goldenFreddyRoomScheme.ForceMoveNamed(room);
    }
    
    public void SetPhone(bool enable)
    {
        Debug.Log("Set phone " + enable);

        if (enable)
        {
            onIncomingCall.Invoke();
            StartCoroutine(StopAndAcceptCallLater());
        }
        else
        {
            onStopCall.Invoke();
        }
    }

    public void SendBackBonnie()
    {
        RoomConnection room = bonnieRoomScheme.GetRandomStartingRoom();
        //SoundEffects.Play("KnockLeft");
        SendMove("Bonnie", room.roomName, true);
    }

    public void SendBackChica()
    {
        RoomConnection room = chicaRoomScheme.GetRandomStartingRoom();
        //SoundEffects.Play("KnockRight");
        SendMove("Chica", room.roomName, true);
    }

    public void SendBackFreddy()
    {
        RoomConnection room = freddyRoomScheme.GetRandomStartingRoom();
        //SoundEffects.Play("KnockRight");
        SendMove("Freddy", room.roomName, true);
    }

    public void SendBackFoxy()
    {
        RoomConnection room = foxyRoomScheme.GetRandomStartingRoom();
        //SoundEffects.Play("KnockLeft");
        SendMove("Foxy", room.roomName, true);
    }

    public void SendBackGoldenFreddy()
    {
        if (goldenFreddyRoomScheme.GetCurrentRoom().roomName != "Office" || !NightSetup.isGuard)
            return;

        RoomConnection room = goldenFreddyRoomScheme.GetRandomStartingRoom();
        SendOfficeChange("SendBackGoldenFreddy", room.roomName);
    }

    public void SendDoor(string value)
    {
        SendOfficeChange("Door", value);
    }

    public void SendLight(string value)
    {
        SendOfficeChange("Light", value);
    }

    public void SetDoor(string value)
    {
        switch(value)
        {
            case "LOPEN":
                leftDoor.JumpFirst();
                break;
            case "LCLOSE":
                leftDoor.Jump(1);
                break;
            case "ROPEN":
                rightDoor.JumpFirst();
                break;
            case "RCLOSE":
                rightDoor.Jump(1);
                break;
        }
    }

    public void SetLight(string value)
    {
        switch (value)
        {
            case "LON":
                office.Jump(2);
                SoundEffects.Play("LightLeft");
                break;
            case "LOFF":
                office.JumpFirst();
                SoundEffects.Stop("LightLeft");
                break;
            case "RON":
                office.Jump(1);
                SoundEffects.Play("LightRight");
                break;
            case "ROFF":
                office.JumpFirst();
                SoundEffects.Stop("LightRight");
                break;
        }
    }

    public void SendLookingMapBlib(string value)
    {
        if (NightSetup.isGuard)
            SendOfficeChange("LookingMapBlib", value);
    }

    public void SetLookingMapBlib(string value)
    {
        int i = int.Parse(value);

        if (NightSetup.isGuard)
            return;

        if (NightSetup.isObserver)
        {
            if (i >= 11)
            {
                cameras.Close();
            }
            else
            {
                cameras.Open();

                cameraButtons.transform.GetChild(i).GetComponent<ClickEvent>().clickEvent?.Invoke();
            }
        }

        if (NightSetup.isAfton)
        {
            if (i < 0)
                securityGuardBlibs.JumpLast();
            else
                securityGuardBlibs.Jump(i);
        }
    }

    public void SendZeroPower()
    {
        SendOfficeChange("ZeroPower", true);
    }

    public void SetZeroPower()
    {
        onPowerZeroNotSecurity.Invoke();
        //night.PowerDown();
    }

    public void SendPhone(bool enable)
    {
        SendHack("Phone", enable);
        SetPhone(enable);
    }

    public void SendAddPower(int powerToSend)
    {
        SendHack("AddPower", powerToSend);
    }

    public void SetAddPower(int power)
    {
        night.AddPower(power);
    }

    public void SendItsMeDistraction()
    {
        SendHack("ItsMeDistraction", bool.TrueString);
    }

    public void SetItsMeDistraction()
    {
        if (onDistract != null)
            onDistract.Invoke();
    }

    private IEnumerator StopAndAcceptCallLater()
    {
        yield return new WaitForSeconds(13f);
        onCallAccepted.Invoke();
        yield return new WaitForSeconds(60f);
        onStopCall.Invoke();
    }

    /*public void SendReturnToLobby()
    {
        SendGameCourse("ReturnToLobby");
    }*/

    /*public void SendExit()
    {
        network.Send("EXIT", 0.ToString());
    }

    public void SetExit(string value)
    {
        if (onExit != null)
            onExit.Invoke();
    }*/

    /*public void SendEndNight()
    {
        network.Send("END", 0.ToString());
    }

    public void SetEndNight()
    {
        night.EndNight();
    }*/

    /*public void SendFoxyScare()
    {
        if (!LocalDataExtractor.isSecurity)
            return;

        network.Send("SCAREFOXY", 0.ToString());
    }

    public void SendGoldenFreddyScare()
    {
        if (!LocalDataExtractor.isSecurity)
            return;

        network.Send("SCAREGOLDENFREDDY", 0.ToString());
    }*/

    /*public void SendCollectedCoins(bool onlyIfSecurity)
    {
        if (onlyIfSecurity != LocalDataExtractor.isSecurity)
            return;

        int coins = Mathf.CeilToInt(Stats.j.GetInt("collectedCoins") * Random.Range(0.4f,1f));

        Stats.j.Increment("stolenCoins", -coins);

        if (coins <= 0)
            return;

        network.Send("COINS", coins.ToString());
    }

    public void SetCollectedCoins(string value)
    {
        int coins = 0;
        int.TryParse(value, out coins);

        Stats.j.Increment("stolenCoins", coins);
    }*/
}
