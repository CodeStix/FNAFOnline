using FNAFOnline.Shared;
using Stx.Collections.Concurrent;
using Stx.Logging;
using Stx.Net;
using Stx.Net.RoomBased;
using Stx.Net.ServerOnly;
using Stx.Net.ServerOnly.RoomBased;
using Stx.Serialization;
using Stx.Utilities;
using System.Collections;
using System.Collections.Concurrent;

namespace FNAFOnline.Server
{
    public class FNAFRoom_Server : ServerPlugin
    {
        public override string PluginName { get; } = "FNAFRoom";
        public override string PluginVersion { get; } = "1.0";

        public override void OnEnable()
        {
            base.OnEnable();

            GameSetup.RegisterNetworkTypes();

            Server.Matchmaking.OnNewRoom += Matchmaking_OnNewRoom;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            Server.Matchmaking.OnNewRoom -= Matchmaking_OnNewRoom;
        }

        private void Matchmaking_OnNewRoom(ServerRoom room, string arg2)
        {
            room.Application = new FNAF1Game();
        }




        // About tags:
        // Afton = the person who controls the animatronics.
        // Guard = the person who must protect himself from the animatronics.
        // Others (without tags) receive the data and can only observe.

        //private ConcurrentDictionary<string, GameSetup> gameSetups = new ConcurrentDictionary<string, GameSetup>();
        //private ConcurrentDictionary<string, ConcurrentList<string>> loaded = new ConcurrentDictionary<string, ConcurrentList<string>>();

        /* public override void OnEnable()
         {
             Bytifier.IncludeManually<GameSetup>(100);
             Bytifier.IncludeManually<GameEndInfo>(101);
             Bytifier.IncludeManually<MoveTimings>(102);
             Bytifier.IncludeManually<GameEndCause>(103);

             Server.Matchmaking.OnRoomDestroyed += ServerMatchmaking_OnRoomDestroyed;
             Server.Matchmaking.OnNewRoom += ServerMatchmaking_OnNewRoom;
             Server.Matchmaking.OnGameCanStart += Server_OnGameCanStart;*/

        // Request that afton sends to everyone to indicate that he has moved an animatronic.
        /*Server.RequestHandlers.Add("FNAFMoveEntity", (packet, sender) =>
        {
            Logger.Log("FNAFMoveEntity request received.", LoggedImportance.Debug);

            if (!sender.Identity.IsInRoom())
                return;

            ServerRoom r = Server.Matchmaking.GetRoomFromID(sender.ClientIdentity.RoomID);

            Logger.Log("Checking if valid...", LoggedImportance.Debug);

            if (r == null || r.Underlaying.State != GameState.InGame || !r.Underlaying.IsClientTaggedWith(sender.ClientID, "Afton"))
                return;

            Logger.Log("Checking for data...", LoggedImportance.Debug);

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

            Logger.Log($"Broadcasting move with { entity } to { to }({ back }) ...", LoggedImportance.Debug);

            Server.Matchmaking.BroadcastInRoom(r.ID, sender.ClientID, moveData, false);

            packet.ResponseOk();
        });*/

        // Request that afton sends to everyone(but himself) to indicate that he used a special hack.
        /*Server.RequestHandlers.Add("FNAFHack", (packet, sender) =>
        {
            if (!sender.ClientIdentity.IsInRoom())
                return;

            ServerRoom r = Server.Matchmaking.GetRoomFromID(sender.ClientIdentity.RoomID);

            if (r == null || r.Underlaying.State != GameState.InGame || !r.Underlaying.IsClientTaggedWith(sender.ClientID, "Afton"))
                return;

            Hashtable data = new Hashtable();
            data.Add("FNAFHacked", true);

            if (packet.Data.Requires<int>("AddPower"))
                data.Add("AddPower", packet.Data["AddPower"]);
            if (packet.Data.Contains("ItsMeDistraction"))
                data.Add("ItsMeDistraction", packet.Data["ItsMeDistraction"]);

            Server.Matchmaking.BroadcastInRoom(r.ID, sender.ClientID, data, true);

            packet.ResponseOk();
        });*/

        // Request that the guard sends to everyone to indicate that something in the office has changed.
        /*Server.RequestHandlers.Add("FNAFOffice", (packet, sender) =>
        {
            if (!sender.ClientIdentity.IsInRoom())
                return;

            ServerRoom r = Server.Matchmaking.GetRoomFromID(sender.ClientIdentity.RoomID);

            if (r == null || r.Underlaying.State != GameState.InGame || !r.Underlaying.IsClientTaggedWith(sender.ClientID, "Guard"))
                return;

            Hashtable data = new Hashtable();
            data.Add("FNAFOfficeChanged", true);

            if (packet.Data.Requires<string>("Door"))
                data.Add("Door", packet.Data["Door"]);
            else if (packet.Data.Requires<string>("Light"))
                data.Add("Light", packet.Data["Light"]);
            else if (packet.Data.Requires<string>("LookingMapBlib"))
                data.Add("LookingMapBlib", packet.Data["LookingMapBlib"]);
            //else if (packet.Data.Requires<int>("Power"))
            //    data.Add("Power", packet.Data["Power"]);
            else if (packet.Data.ContainsKey("ZeroPower"))
                data.Add("ZeroPower", true);
            else if (packet.Data.Requires<string>("SendBackGoldenFreddy"))
                data.Add("SendBackGoldenFreddy", packet.Data["SendBackGoldenFreddy"]);

            Server.Matchmaking.BroadcastInRoom(r.ID, sender.ClientID, data, false);

            packet.ResponseOk();
        });*/

        // Send by a player that needs the setup object.
        /*Server.RequestHandlers.Add("FNAFRequireSetup", (packet, sender) =>
        {
            Logger.Log("A setup was required.", LoggedImportance.Debug);

            Logger.Log(sender.ClientID + " needs setup object.", LoggedImportance.Debug);

            if (!sender.ClientIdentity.IsInRoom())
                return;

            ServerRoom r = Server.Matchmaking.GetRoomFromID(sender.ClientIdentity.RoomID);

            if (r == null || r.Underlaying.State != GameState.InGame)
                return;

            Logger.Log(sender.ClientID + " is loaded.", LoggedImportance.Debug);

            if (loaded.ContainsKey(r.ID))
            {
                if (!loaded[r.ID].Contains(sender.ClientID))
                    loaded[r.ID].Add(sender.ClientID);

                //DebugHandler.Info("Checking if everyone is loaded... Loaded clients: " + loaded[r].Count + "; In room: " + r.PlayerCount);



                if (loaded[r.ID].Count >= r.Underlaying.PlayerCount)
                {
                    ConcurrentList<string> _;
                    loaded.TryRemove(r.ID, out _);

                    Hashtable data = new Hashtable();
                    data.Add("FNAFEveryoneLoaded", true);
                    Server.Matchmaking.BroadcastInRoom(r.ID, sender.ClientID, data, false);

                    Logger.Log("Everyone is loaded!", LoggedImportance.Debug);
                }
            }

            Logger.Log("Send setup object to " + sender.ClientID, LoggedImportance.Debug);

            packet.Respond(gameSetups[r.ID]);
        });*/

        // Send by a player that (for example) needs to enter the pause menu.
        /* Server.RequestHandlers.Add("FNAFGameCourse", (packet, sender) =>
         {
             if (!sender.ClientIdentity.IsInRoom())
                 return;

             ServerRoom r = Server.Matchmaking.GetRoomFromID(sender.ClientIdentity.RoomID);

             if (r == null || r.Underlaying.State != GameState.InGame)
                 return;

             //Hashtable data = new Hashtable();
             //data.Add("FNAFGameCourse", true);

             if (packet.Data.Requires<GameEndInfo>("EndGame"))
             {
                 GameEndInfo gei = (GameEndInfo)packet.Data["EndGame"];

                 Logger.Log(sender.ToIdentifiedString() + " wants to end the game.", LoggedImportance.Debug);

                 EndGame(r, gei);
             }

             //Server.ServerMatchmaking.BroadcastInRoom(r.ID, sender.ClientID, data, false);

             packet.ResponseOk();
         });
     }*/

        /* private void Server_OnGameCanStart(ServerRoom room)
         {
             Logger.Log("Setting roles for room " + room.Underlaying.Name, LoggedImportance.Debug);

             GameSetup gs = gameSetups[room.ID];

             var c = room.Underlaying.GetClientsInRandomOrder();
             string guardClientID = c[0];
             string aftonClientID = c[1];

             gs.SetPlayerRoles(guardClientID, aftonClientID);

             room.Underlaying.UnTagClients("Guard");
             room.Underlaying.UnTagClients("Afton");
             room.Underlaying.TagClient(guardClientID, "Guard");
             room.Underlaying.TagClient(aftonClientID, "Afton");

             if (loaded.ContainsKey(room.ID))
             {
                 ConcurrentList<string> _;
                 loaded.TryRemove(room.ID, out _);
             }

             loaded.TryAdd(room.ID, new ConcurrentList<string>());

             if (!StartGame(room))
                 Logger.Log("A game could not get started!", LoggedImportance.CriticalError);
             else
                 Logger.Log("A game was started.", LoggedImportance.Successful);
         }*/

        /*public override void OnDisable()
        {
            Server.RequestHandlers.Remove("FNAFMoveEntity");
            Server.RequestHandlers.Remove("FNAFOfficeChanged");
            Server.RequestHandlers.Remove("FNAFHack");
            Server.RequestHandlers.Remove("FNAFRequireSetup");
            Server.RequestHandlers.Remove("FNAFGameCourse");

            Server.Matchmaking.OnRoomDestroyed -= ServerMatchmaking_OnRoomDestroyed;
            Server.Matchmaking.OnNewRoom -= ServerMatchmaking_OnNewRoom;
            Server.Matchmaking.OnGameCanStart -= Server_OnGameCanStart;
        }*/

        /*private void ServerMatchmaking_OnNewRoom(ServerRoom room, string creator)
        {
            room.Bound = new FNAFGame();
            room.Underlaying.RequiredPlayerCount = 2;

            // Create a default game setup with default move timings and unset player roles.
            gameSetups.TryAdd(room.ID, new GameSetup(new MoveTimings()));
        }*/

        /*private void ServerMatchmaking_OnRoomDestroyed(ServerRoom room)
        {
            Logger.Log("The room " + room.Underlaying.Name + " was destroyed.", LoggedImportance.Debug);

            ConcurrentList<string> _;
            loaded.TryRemove(room.ID, out _);

            GameSetup __;
            gameSetups.TryRemove(room.ID, out __);
        }*/

        /*public bool StartGame(ServerRoom forRoom)
        {
            if (forRoom.Underlaying.State == GameState.InGame)
                return false;

            GameSetup gs = gameSetups[forRoom.ID];

            Server.Scheduler.RunLater(() =>
            {
                Logger.Log("Test: 4 seconds after game was started.", LoggedImportance.Debug);
            }, 4000);

            return Server.Matchmaking.ChangeGameState(forRoom.ID, GameState.InGame);
        }

        public bool EndGame(ServerRoom forRoom, GameEndInfo endInfo)
        {
            if (forRoom.Underlaying.State != GameState.InGame)
                return false;

            Logger.Log("Game has ended for room " + forRoom.Underlaying.Name + ": " + endInfo, LoggedImportance.Successful);

            return Server.Matchmaking.ChangeGameState(forRoom.ID, GameState.InLobby);
        }*/
    }
}
