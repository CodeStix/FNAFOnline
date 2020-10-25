using FNAFOnline.Shared;
using Stx.Collections.Concurrent;
using Stx.Logging;
using Stx.Net;
using Stx.Net.RoomBased;
using Stx.Net.ServerOnly;
using Stx.Net.ServerOnly.RoomBased;
using Stx.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNAFOnline.Server
{
    // Every hour lasts 85 seconds.

    public class FNAFGame : RoomApplication
    {
        public GameSetup gameSetup;

        private int totalClientsLoaded = 0;

        public override void OnInitialize()
        {
            RegisterHandler("FNAFLoaded", RequestLoaded);

            gameSetup = new GameSetup(GameMode.None, new MoveTimings());
        }

        public override void OnStart()
        {  
            totalClientsLoaded = 0;
        }

        private void RequestLoaded(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (++totalClientsLoaded >= Room.PlayerCount)
            {
                totalClientsLoaded = int.MinValue;

                ServerRoom.BroadcastAsServer(new Hashtable()
                {
                    ["FNAFStart"] = gameSetup
                });
            }

            packet.ResponseOk();
        }
    }

    public class FNAF1Game : FNAFGame
    {
        private float powerLeft = 100f;
        private string powerDrainTaskId;

        private bool isLeftDoorClosed = false;
        private bool isRightDoorClosed = false;
        private bool isLeftLightOn = false;
        private bool isRightLightOn = false;
        private short currentCamera = -1; // -1 = not looking at any camera, monitor down

        public override void OnInitialize()
        {
            base.OnInitialize();

            RegisterHandler("FNAF1Move", RequestMove);
            RegisterHandler("FNAF1Office", RequestOffice);
           
            gameSetup = new GameSetup(GameMode.OriginalFNAF1, new MoveTimings());
        }

        public override void OnCanStart()
        {
            var c = Room.GetClientsInRandomOrder();
            string guardClientID = c[0];
            string aftonClientID = c[1];

            gameSetup.SetPlayerRoles(guardClientID, aftonClientID);

            Room.UnTagClients("Guard");
            Room.UnTagClients("Afton");
            Room.TagClient(guardClientID, "Guard");
            Room.TagClient(aftonClientID, "Afton");

            ChangeState(GameState.InGame);
        }

        public override void OnStart()
        {
            base.OnStart();

            Logger.Info("Game is starting...");
            //System.Diagnostics.Debug.Assert(false);

            powerLeft = gameSetup.StartingPower;

            powerDrainTaskId = Server.Scheduler.Repeat(() =>
            {
                int powerUsage = 1;

                if (isLeftDoorClosed)
                    powerUsage++;
                if (isRightDoorClosed)
                    powerUsage++;
                if (isLeftLightOn)
                    powerUsage++;
                if (isRightLightOn)
                    powerUsage++;
                if (currentCamera > -1)
                    powerUsage++;

                int previous = (int)powerLeft;

                if (powerUsage == 1)
                    powerLeft -= 0.141f;
                else if (powerUsage == 2)
                    powerLeft -= 0.235f;
                else if (powerUsage == 3)
                    powerLeft -= 0.341f;
                else if (powerUsage == 4)
                    powerLeft -= 0.447f;
                else if (powerUsage >= 5)
                    powerLeft -= 0.669f;

                if (previous != (int)powerLeft)
                {
                    Logger.Debug($"Power drain task send update: usage: { powerUsage }, left: { powerLeft } (LD: { isLeftDoorClosed }, RD: { isRightDoorClosed }, LL: { isLeftLightOn }, RL: { isRightLightOn })");

                    ServerRoom.BroadcastAsServer(new Hashtable()
                    {
                        ["FNAF1Power"] = (int)powerLeft
                    });
                }

            }, 1000).TaskID;
        }

        public override void OnEnd()
        {
            Server.Scheduler.StopRun(powerDrainTaskId);
        }

        public override void OnTerminate()
        {
            Server.Scheduler.StopRun(powerDrainTaskId);
        }

        private void RequestMove(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (State != GameState.InGame || !Room.IsClientTaggedWith(sender.NetworkID, "Afton") || !packet.Data.Requires<MonsterType, string>("Entity", "Destination"))
                return;

            ServerRoom.BroadcastAsServer(new Hashtable()
            {
                ["FNAF1Moved"] = true,
                ["Entity"] = (MonsterType)packet.Data["Entity"],
                ["Destination"] = (string)packet.Data["Destination"]
            });

            packet.ResponseOk();
        }

        private void RequestOffice(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (State != GameState.InGame/* || !Room.IsClientTaggedWith(sender.NetworkID, "Guard")*/)
                return;

            if (packet.Data.Requires<short>("CurrentCamera"))
                currentCamera = (short)packet.Data["CurrentCamera"];
            else if (packet.Data.Requires<bool>("DoorR"))
                isRightDoorClosed = (bool)packet.Data["DoorR"];
            else if (packet.Data.Requires<bool>("DoorL"))
                isLeftDoorClosed = (bool)packet.Data["DoorL"];
            else if (packet.Data.Requires<bool>("LightR"))
                isRightLightOn = (bool)packet.Data["LightR"];
            else if (packet.Data.Requires<bool>("LightL"))
                isLeftLightOn = (bool)packet.Data["LightL"];
            else
                return;

            ServerRoom.BroadcastAsServer(new Hashtable()
            {
                ["FNAF1Office"] = true,
                ["CurrentCamera"] = currentCamera,
                ["DoorR"] = isRightDoorClosed,
                ["DoorL"] = isLeftDoorClosed,
                ["LightR"] = isRightLightOn,
                ["LightL"] = isLeftLightOn
            });

            packet.ResponseOk();
        }
    }


    /*public class OldFNAFGame : RoomApplication
    {
        private ConcurrentList<string> loaded;
        private GameSetup gameSetup;

        public override void OnInitialize()
        {
            RegisterHandler("FNAFMoveEntity", RequestMoveEntity);
            RegisterHandler("FNAFHack", RequestHack);
            RegisterHandler("FNAFOffice", RequestOffice);
            RegisterHandler("FNAFRequireSetup", RequestRequireSetup);
            RegisterHandler("FNAFGameCourse", RequestGameCourse);

            loaded = new ConcurrentList<string>();
            gameSetup = new GameSetup(GameMode.None,new MoveTimings());
        }

        private void RequestMoveEntity(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (State != GameState.InGame || !Room.IsClientTaggedWith(sender.NetworkID, "Afton"))
                return;

            if (!packet.Data.Requires<string, string, bool>("Entity", "To", "Back"))
                return;

            string entity = (string)packet.Data["Entity"];
            string to = (string)packet.Data["To"];
            bool back = (bool)packet.Data["Back"];

            Hashtable moveData = new Hashtable();
            moveData.Add("FNAFEntityMoved", true);
            moveData.Add("Entity", entity);
            moveData.Add("To", to);
            moveData.Add("Back", back);

            Logger.Debug($"Broadcasting move with { entity } to { to }({ back }) ...");

            ServerRoom.BroadcastAsServer(moveData);

            packet.ResponseOk();
        }

        private void RequestHack(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (State != GameState.InGame || !Room.IsClientTaggedWith(sender.NetworkID, "Afton"))
                return;

            Hashtable data = new Hashtable();
            data.Add("FNAFHacked", true);

            if (packet.Data.Requires<int>("AddPower"))
                data.Add("AddPower", packet.Data["AddPower"]);
            if (packet.Data.Contains("ItsMeDistraction"))
                data.Add("ItsMeDistraction", packet.Data["ItsMeDistraction"]);

            ServerRoom.TryBroadcast(sender.NetworkID, data, true);

            packet.ResponseOk();
        }

        private void RequestOffice(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (State != GameState.InGame || !Room.IsClientTaggedWith(sender.NetworkID, "Guard"))
                return;

            Hashtable officeData = new Hashtable();
            officeData.Add("FNAFOfficeChanged", true);

            if (packet.Data.Requires<string>("Door"))
                officeData.Add("Door", packet.Data["Door"]);
            else if (packet.Data.Requires<string>("Light"))
                officeData.Add("Light", packet.Data["Light"]);
            else if (packet.Data.Requires<string>("LookingMapBlib"))
                officeData.Add("LookingMapBlib", packet.Data["LookingMapBlib"]);
            else if (packet.Data.ContainsKey("ZeroPower"))
                officeData.Add("ZeroPower", true);
            else if (packet.Data.Requires<string>("SendBackGoldenFreddy"))
                officeData.Add("SendBackGoldenFreddy", packet.Data["SendBackGoldenFreddy"]);

            ServerRoom.BroadcastAsServer(officeData);

            packet.ResponseOk();
        }

        private void RequestRequireSetup(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (State != GameState.InGame || loaded.Contains(sender.NetworkID))
                return;

            Logger.Log(sender.NetworkID + " is loaded.", LoggedImportance.Debug);

            loaded.Add(sender.NetworkID);

            Logger.Log("Send setup object to " + sender.NetworkID, LoggedImportance.Debug);

            if (loaded.Count >= Room.PlayerCount)
            {
                Hashtable data = new Hashtable();
                data.Add("FNAFEveryoneLoaded", true);
                ServerRoom.BroadcastAsServer(data);

                Logger.Log("Everyone is loaded!", LoggedImportance.Debug);
            }

            packet.Respond(gameSetup);
        }

        private void RequestGameCourse(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (State != GameState.InGame)
                return;

            if (packet.Data.Requires<GameEndInfo>("EndGame"))
            {
                GameEndInfo gei = (GameEndInfo)packet.Data["EndGame"];

                Logger.Log(sender.ToIdentifiedString() + " wants to end the game.", LoggedImportance.Debug);

                ServerRoom.TryChangeGameState(GameState.InLobby);
            }

            packet.ResponseOk();
        }

        public override void OnCanStart()
        {
            StartGame();
        }

        public void StartGame()
        {
            var c = Room.GetClientsInRandomOrder();
            string guardClientID = c[0];
            string aftonClientID = c[1];

            gameSetup.SetPlayerRoles(guardClientID, aftonClientID);

            Room.UnTagClients("Guard");
            Room.UnTagClients("Afton");
            Room.TagClient(guardClientID, "Guard");
            Room.TagClient(aftonClientID, "Afton");

            Server.Scheduler.RunLater(() =>
            {
                Logger.Log("Test: 4 seconds after game was started.", LoggedImportance.Debug);
            }, 4000);

            ServerRoom.TryChangeGameState(GameState.InGame);
        }

        public override void OnEnd()
        {
            loaded.Clear();
        }
    }*/
}
