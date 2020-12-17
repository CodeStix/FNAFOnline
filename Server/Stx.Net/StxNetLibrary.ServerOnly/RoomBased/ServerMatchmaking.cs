using Stx.Logging;
using Stx.Net.RoomBased;
using Stx.Serialization;
using Stx.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stx.Net.ServerOnly.RoomBased
{
    public class ServerMatchmaking
    {
        public Server Server { get; }
        public UniqueCodeGenerator RoomCodeGenerator { get; set; } = new UniqueCodeGenerator();

        protected BConcurrentList<ServerRoom> Rooms { get; set; } = new BConcurrentList<ServerRoom>();

        public event Action<ServerRoom, string> OnNewRoom;
        public event Action<ServerRoom> OnRoomDestroyed;

        public ILogger Logger { get; set; } = StxNet.DefaultLogger;
        public StringComparison RoomCodeComparer { get; set; } = StringComparison.OrdinalIgnoreCase;
        public int RoomCount
        {
            get
            {
                return Rooms.Count;
            }
        }

        public ServerMatchmaking(Server server)
        {
            Server = server;
        }

        public ServerRoom GetRoomWhere(ClientData isClient)
            => GetRoomWhere(isClient.Identity);

        public ServerRoom GetRoomWhere(ClientIdentity isClient)
        {
            if (!isClient.IsInAnyRoom)
                return null;

            return GetServerRoom(isClient.RoomID);
        }

        public ServerRoom GetServerRoom(string roomID)
        {
            lock(Rooms.Locker)
            {
                return Rooms.FirstOrDefault((e) => e.ID == roomID);
            }
        }

        public Room GetRoom(string roomID)
        {
            lock (Rooms.Locker)
            {
                return Rooms.FirstOrDefault((e) => e.ID == roomID)?.Underlaying;
            }
        }

        public ServerRoom GetRoomWithCode(string roomCode)
        {
            lock (Rooms.Locker)
            {
                return Rooms.FirstOrDefault((e) => string.Compare(e.Underlaying.RoomCode, roomCode, RoomCodeComparer) == 0);
            }
        }

        /// <summary>
        /// Performs a query on all the rooms on the server and returns matching rooms.
        /// </summary>
        /// <param name="query">The query all returned rooms should match.</param>
        /// <param name="forceReturnAllMatches">True if you want to return all the results that match the query and skip the <see cref="MatchmakingQuery.Page"/> and <see cref="MatchmakingQuery.ResultsPerPage"/> parameters.</param>
        /// <returns></returns>
        public MatchmakingQueryResult GetMatches(MatchmakingQuery query, bool forceReturnAllMatches = false)
        {
            MatchmakingQueryResult mqr = new MatchmakingQueryResult();

            if (query == null)
                return mqr;

            if (query.ResultsPerPage > MatchmakingQuery.MaxResults)
                query.ResultsPerPage = MatchmakingQuery.MaxResults;

            var rooms = Rooms.SafeCopy();
            var matches = rooms.Where((e) =>
                (string.IsNullOrEmpty(query.RequiredRoomCode) || string.Compare(e.Underlaying.RoomCode, query.RequiredRoomCode, RoomCodeComparer) == 0)
                && (string.IsNullOrEmpty(query.MatchedName) || e.Underlaying.Name.ToUpper().Contains(query.MatchedName.ToUpper()))
                && (!e.Underlaying.IsFull || !query.OnlyNotFull)
                && (!e.Underlaying.Locked || !query.OnlyUnlocked)
                && (!e.Hidden)
                && (string.IsNullOrEmpty(query.MatchedID) || e.ID == query.MatchedID)
                && (query.GameState == null || e.Underlaying.State == query.GameState)
                && (string.IsNullOrWhiteSpace(query.RequiredRoomTag) || e.Underlaying.IsRoomTaggedWith(query.RequiredRoomTag)));

            mqr.TotalRooms = rooms.Count;
            mqr.MatchedRooms = matches.Count();

            if (forceReturnAllMatches)
            {
                mqr.Rooms = new BList<Room>(matches.Select((r) => r.Underlaying));
            }
            else
            {
                mqr.Rooms = new BList<Room>(matches
                    .Select((r) => r.Underlaying)
                    .Skip(query.Page * query.ResultsPerPage)
                    .Take(query.ResultsPerPage));
            }

            return mqr;
        }

        public void RemoveUnusedRooms()
        {
            lock (Rooms.Locker)
            {
                Rooms.Where((room) => room.ShouldBeDisposed).ToList().ForEach((room) =>
                {
                    DestroyRoomWithID(room.ID);

                    Logger.Log("Removed unused room: " + room.ToString(), LoggedImportance.Debug);
                });
            }
        }

        public bool LetBroadcastInRoom(ClientData client, Hashtable data, bool excludeSender = true, params string[] receivers)
        {
            if (!client.Identity.IsInAnyRoom)
                return false;

            ServerRoom room = GetRoomWhere(client);

            // A client-send broadcast cannot contain an underscore!
            foreach (string key in data.Keys)
                if (string.IsNullOrWhiteSpace(key) || key.Contains("_"))
                    return false;

            if (room == null)
                return false;

            return room.TryBroadcast(client.NetworkID, data, excludeSender, receivers);
        }

        public ServerRoom LetLeaveCurrentRoom(ClientData client)
        {
            if (!client.Identity.IsInAnyRoom)
                return null;

            ServerRoom a = GetServerRoom(client.Identity.RoomID);

            if (a == null)
            {
                client.Identity.CamePreLobby();

                return null;
            }

            if (a.TryRemove(client))
            {
                if (a.ShouldBeDisposed)
                    DestroyRoom(a);

                return a;
            }

            return null;
        }

        public ServerRoom LetJoinNewRoom(ClientData client, RoomTemplate template)
        {
            if (template.IsValid() && !client.Identity.IsInAnyRoom)
            {
                ServerRoom room = new ServerRoom(Server, template, client.Identity, RoomCodeGenerator.GetNextCode());
                Rooms.Add(room);

                OnNewRoom?.Invoke(room, client.NetworkID);

                room.ForceJoin(client);

                return room;
            }

            return null;
        }

        public ServerRoom LetJoinRandomRoom(ClientData client, MatchmakingQuery mustMatch)
        {
            if (mustMatch == null)
                return null;

            mustMatch.OnlyUnlocked = true;
            mustMatch.OnlyNotFull = true;
            mustMatch.GameState = GameState.InLobby;

            Room r = GetMatches(mustMatch, true).MostSuitableResult;

            if (r != null)
                return LetJoinRoom(client, r.ID);

            return null;
        }

        public ServerRoom LetJoinRandomOrNewRoom(ClientData client, MatchmakingQuery mustMatch, RoomTemplate fallbackTemplate)
        {
            ServerRoom r = LetJoinRandomRoom(client, mustMatch);

            if (r == null)
                r = LetJoinNewRoom(client, fallbackTemplate);

            return r;
        }

        public ServerRoom LetJoinRoomWithCode(ClientData client, string roomCode, string password = null)
        {
            ServerRoom r = GetRoomWithCode(roomCode);

            if (r != null)
                r = LetJoinRoom(client, r, password);

            return r;
        }

        public ServerRoom LetJoinRoom(ClientData client, string roomID, string password = null)
        {
            ServerRoom r = GetServerRoom(roomID);

            if (r != null)
                r = LetJoinRoom(client, r, password);

            return r;
        }

        public ServerRoom LetJoinRoom(ClientData client, ServerRoom room, string password = null)
            => room.TryJoin(client, password) ? room : null;
            
        public ServerRoom CreateNewRoom(RoomTemplate template)
        {
            if (!template.IsValid())
                return null;

            ServerRoom room = new ServerRoom(Server, template, Server, RoomCodeGenerator.GetNextCode());
            Rooms.Add(room);

            OnNewRoom?.Invoke(room, Server.NetworkID);

            return room;
        }

        public ServerRoom LetCreateNewRoom(ClientData client, RoomTemplate template)
        {
            if (template.IsValid() && !client.Identity.IsInAnyRoom)
            {
                ServerRoom room = new ServerRoom(Server, template, client.Identity, RoomCodeGenerator.GetNextCode());
                Rooms.Add(room);

                OnNewRoom?.Invoke(room, client.NetworkID);

                return room;
            }

            return null;
        }

        public ClientRoomStatus LetChangeClientRoomStatus(ClientData client, ClientRoomStatus newStatus)
        {
            if (client == null)
                return ClientRoomStatus.None;

            ServerRoom r = GetServerRoom(client.Identity.RoomID);

            if (r == null)
                return ClientRoomStatus.None;

            client.Identity.RoomStatus = newStatus;
            client.Save();

            r.TryBroadcast(client.NetworkID, new Hashtable()
            {
                ["_SomeoneChangedStatus"] = newStatus,
                ["WhoChanged"] = client.Identity
            }, false);

            // Checking if everyone is ready => game can start.
            if (r.Underlaying.State == GameState.InLobby)
            {
                bool canStart = true;
                r.Underlaying.ConnectedClients.ForEach((c) =>
                {
                    if (Server.GetConnectedClient(c)?.Identity.RoomStatus != ClientRoomStatus.Ready)
                        canStart = false;
                });

                //Logger.Log($"Checking if game can start. (Enough players: { r.Underlaying.EnoughPlayers }, Players ready: { canStart })", LoggedImportance.Debug);
                if (r.Underlaying.EnoughPlayers && canStart)
                    r.Application.OnCanStart();
            }

            return newStatus;
        }

        public bool DestroyRoom(ServerRoom room)
        {
            if (room == null)
                return false;

            room.Application.OnTerminate();
            room.Server.RemoveAllRoomRequestHandlers(room.ID);
            room.KickAll();

            bool removed = Rooms.Remove(room);

            if (removed)
                OnRoomDestroyed?.Invoke(room);

            return removed;
        }

        public bool DestroyRoomWithID(string roomID)
        {
            ServerRoom r = GetServerRoom(roomID);

            return DestroyRoom(r);
        }

        public void KickFromCurrent(ClientIdentity client)
        {
            if (!client.IsInAnyRoom)
                return;

            ServerRoom room = GetServerRoom(client.RoomID);

            if (room == null)
            {
                client.CamePreLobby();

                return;
            }

            room.KickAsServer(client.NetworkID);

            if (room.ShouldBeDisposed)
                DestroyRoom(room);
        }

        public List<ServerRoom> GetAllRooms()
        {
            return Rooms.SafeCopy();
        }
    }
}
