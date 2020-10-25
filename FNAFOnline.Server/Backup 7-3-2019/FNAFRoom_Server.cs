using FNAFOnline.Shared;
using Stx.Collections.Concurrent;
using Stx.Logging;
using Stx.Net.Rooms;
using Stx.Net.ServerOnly;
using Stx.Serialization;
using Stx.Utilities;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FNAFOnline.Server
{
    public class FNAFGame
    {

    }

    public class FNAFRoom_Server : ServerPlugin
    {
        public override string PluginName { get; } = "FNAFRoom";
        public override string PluginVersion { get; } = "1.0";

        // About tags:
        // Afton = the person who controls the animatronics.
        // Guard = the person who must protect himself from the animatronics.
        // Others (without tags) receive the data and can only observe.

        private ConcurrentDictionary<Room, GameSetup> gameSetups = new ConcurrentDictionary<Room, GameSetup>();
        private ConcurrentDictionary<Room, ConcurrentList<string>> loaded = new ConcurrentDictionary<Room, ConcurrentList<string>>();

        public override void OnEnable()
        {
            Bytifier.IncludeManually<GameSetup>(100);
            Bytifier.IncludeManually<GameEndInfo>(101);
            Bytifier.IncludeManually<MoveTimings>(102);
            Bytifier.IncludeManually<GameEndCause>(103);

            Server.Matchmaking.OnRoomDestroyed += ServerMatchmaking_OnRoomDestroyed;
            Server.Matchmaking.OnNewRoom += ServerMatchmaking_OnNewRoom;
            Server.Matchmaking.OnGameCanStart += Server_OnGameCanStart;

            // Request that afton sends to everyone to indicate that he has moved an animatronic.
            Server.RequestHandlers.Add("FNAFMoveEntity", (packet, sender) =>
            {
                Logger.Log("FNAFMoveEntity request received.", LoggedImportance.Debug);

                if (!sender.ClientIdentity.IsInRoom())
                    return;

                Room r = Server.Matchmaking.GetRoomFromID(sender.ClientIdentity.RoomID);

                Logger.Log("Checking if valid...", LoggedImportance.Debug);

                if (r == null || r.Info.State != GameState.InGame || !r.Info.IsTaggedWith(sender.ClientID, "Afton"))
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

                Logger.Log("Broadcasting move with " + entity + " to " + to + $"({ back }) ...", LoggedImportance.Debug);

                Server.Matchmaking.BroadcastInRoom(r.ID, sender.ClientID, moveData, false);

                packet.ResponseOk();
            });

            // Request that afton sends to everyone(but himself) to indicate that he used a special hack.
            Server.RequestHandlers.Add("FNAFHack", (packet, sender) =>
            {
                if (!sender.ClientIdentity.IsInRoom())
                    return;

                Room r = Server.Matchmaking.GetRoomFromID(sender.ClientIdentity.RoomID);

                if (r == null || r.Info.State != GameState.InGame || !r.Info.IsTaggedWith(sender.ClientID, "Afton"))
                    return;

                Hashtable data = new Hashtable();
                data.Add("FNAFHacked", true);

                if (packet.Data.Requires<int>("AddPower"))
                    data.Add("AddPower", packet.Data["AddPower"]);
                if (packet.Data.Contains("ItsMeDistraction"))
                    data.Add("ItsMeDistraction", packet.Data["ItsMeDistraction"]);

                Server.Matchmaking.BroadcastInRoom(r.ID, sender.ClientID, data, true);

                packet.ResponseOk();
            });

            // Request that the guard sends to everyone to indicate that something in the office has changed.
            Server.RequestHandlers.Add("FNAFOffice", (packet, sender) =>
            {
                if (!sender.ClientIdentity.IsInRoom())
                    return;

                Room r = Server.Matchmaking.GetRoomFromID(sender.ClientIdentity.RoomID);

                if (r == null || r.Info.State != GameState.InGame || !r.Info.IsTaggedWith(sender.ClientID, "Guard"))
                    return;

                Hashtable data = new Hashtable();
                data.Add("FNAFOfficeChanged", true);

                if (packet.Data.Requires<string>("Door"))
                    data.Add("Door", packet.Data["Door"]);
                else if (packet.Data.Requires<string>("Light"))
                    data.Add("Light", packet.Data["Light"]);
                else if (packet.Data.Requires<string>("LookingMapBlib"))
                    data.Add("LookingMapBlib", packet.Data["LookingMapBlib"]);
                /*else if (packet.Data.Requires<int>("Power"))
                    data.Add("Power", packet.Data["Power"]);*/
                else if (packet.Data.ContainsKey("ZeroPower"))
                    data.Add("ZeroPower", true);
                else if (packet.Data.Requires<string>("SendBackGoldenFreddy"))
                    data.Add("SendBackGoldenFreddy", packet.Data["SendBackGoldenFreddy"]);

                Server.Matchmaking.BroadcastInRoom(r.ID, sender.ClientID, data, false);

                packet.ResponseOk();
            });

            // Send by a player that needs the setup object.
            Server.RequestHandlers.Add("FNAFRequireSetup", (packet, sender) =>
            {
                Logger.Log("A setup was required.", LoggedImportance.Debug);

                Logger.Log(sender.ClientID + " needs setup object.", LoggedImportance.Debug);

                if (!sender.ClientIdentity.IsInRoom())
                    return;

                Room r = Server.Matchmaking.GetRoomFromID(sender.ClientIdentity.RoomID);

                if (r == null || r.Info.State != GameState.InGame)
                    return;

                Logger.Log(sender.ClientID + " is loaded.", LoggedImportance.Debug);

                if (loaded.ContainsKey(r))
                {
                    if (!loaded[r].Contains(sender.ClientID))
                        loaded[r].Add(sender.ClientID);

                    //DebugHandler.Info("Checking if everyone is loaded... Loaded clients: " + loaded[r].Count + "; In room: " + r.PlayerCount);

                 
                    /*data.Add("Everyone", everyone);
                    data.Add("Count", r.PlayerCount);*/

                    if (loaded[r].Count >= r.PlayerCount)
                    {
                        ConcurrentList<string> _;
                        loaded.TryRemove(r, out _);

                        Hashtable data = new Hashtable();
                        data.Add("FNAFEveryoneLoaded", true);
                        Server.Matchmaking.BroadcastInRoom(r.ID, sender.ClientID, data, false);

                        Logger.Log("Everyone is loaded!", LoggedImportance.Debug);
                    }
                }

                Logger.Log("Send setup object to " + sender.ClientID, LoggedImportance.Debug);

                packet.Respond(gameSetups[r]);
            });

            // Send by a player that (for example) needs to enter the pause menu.
            Server.RequestHandlers.Add("FNAFGameCourse", (packet, sender) =>
            {
                if (!sender.ClientIdentity.IsInRoom())
                    return;

                Room r = Server.Matchmaking.GetRoomFromID(sender.ClientIdentity.RoomID);

                if (r == null || r.Info.State != GameState.InGame)
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
        }

        private void Server_OnGameCanStart(Room room)
        {
            Logger.Log("Setting roles for room " + room.Template.Name, LoggedImportance.Debug);

            GameSetup gs = gameSetups[room];

            var c = room.GetClientsInRandomOrder();
            string guardClientID = c[0];
            string aftonClientID = c[1];

            gs.SetPlayerRoles(guardClientID, aftonClientID);

            room.Info.UnTagClients("Guard");
            room.Info.UnTagClients("Afton");
            room.Info.TagClient(guardClientID, "Guard");
            room.Info.TagClient(aftonClientID, "Afton");

            if (loaded.ContainsKey(room))
            {
                ConcurrentList<string> _;
                loaded.TryRemove(room, out _);
            }

            loaded.TryAdd(room, new ConcurrentList<string>());

            if (!StartGame(room))
                Logger.Log("A game could not get started!", LoggedImportance.CriticalError);
            else
                Logger.Log("A game was started.", LoggedImportance.Successful);
        }

        public override void OnDisable()
        {
            Server.RequestHandlers.Remove("FNAFMoveEntity");
            Server.RequestHandlers.Remove("FNAFOfficeChanged");
            Server.RequestHandlers.Remove("FNAFHack");
            Server.RequestHandlers.Remove("FNAFRequireSetup");
            Server.RequestHandlers.Remove("FNAFGameCourse");

            Server.Matchmaking.OnRoomDestroyed -= ServerMatchmaking_OnRoomDestroyed;
            Server.Matchmaking.OnNewRoom -= ServerMatchmaking_OnNewRoom;
            Server.Matchmaking.OnGameCanStart -= Server_OnGameCanStart;
        }

        private void ServerMatchmaking_OnNewRoom(Room room, string creator)
        {
            //DebugHandler.Info("New room is created by " + creator + ", setting up a default GameSetup object.");

            room.RequiredPlayerCount = 2;

            // Create a default game setup with default move timings and unset player roles.
            gameSetups.TryAdd(room, new GameSetup(new MoveTimings()));
        }

        private void ServerMatchmaking_OnRoomDestroyed(Room room)
        {
            Logger.Log("The room " + room.Template.Name + " was destroyed.", LoggedImportance.Debug);

            ConcurrentList<string> _;
            loaded.TryRemove(room, out _);

            GameSetup __;
            gameSetups.TryRemove(room, out __);
        }

        public bool StartGame(Room forRoom)
        {
            if (forRoom.Info.State == GameState.InGame)
                return false;

            GameSetup gs = gameSetups[forRoom];

            Server.Scheduler.RunLater(() =>
            {
                Logger.Log("Test: 4 seconds after game was started.", LoggedImportance.Debug);
            }, 4000);

            return Server.Matchmaking.ChangeGameState(forRoom.ID, GameState.InGame);
        }

        public bool EndGame(Room forRoom, GameEndInfo endInfo)
        {
            if (forRoom.Info.State != GameState.InGame)
                return false;

            Logger.Log("Game has ended for room " + forRoom.DisplayName + ": " + endInfo, LoggedImportance.Successful);

            return Server.Matchmaking.ChangeGameState(forRoom.ID, GameState.InLobby);
        }
    }
}
