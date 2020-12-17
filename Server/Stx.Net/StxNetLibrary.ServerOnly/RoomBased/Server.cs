using Stx.Utilities;
using Stx.PluginArchitecture;
using System;
using System.Net.Sockets;
using System.Collections;
using Stx.Serialization;
using Stx.Logging;
using Stx.Net.RoomBased;
using Stx.Net.Achievements;
using System.Collections.Concurrent;
using System.Collections.Generic;

using RoomHandlerCollection = System.Collections.Concurrent.ConcurrentDictionary<string, System.Collections.Concurrent.ConcurrentDictionary<string, Stx.Net.ServerOnly.RequestDelegate<Stx.Net.RoomBased.ClientIdentity>>>;

namespace Stx.Net.ServerOnly.RoomBased
{
    public class Server : BaseServer<ClientIdentity>, INetworkedItem
    {
        public ServerMatchmaking Matchmaking { get; }
        public AchievementSystem Achievements { get; }
        public PluginLoader<ServerPlugin> PluginLoader { get; }

        public bool EnableGlobalChat { get; set; } = true;

        protected RoomHandlerCollection RequestHandlersPerRoom = new RoomHandlerCollection();

        public Server(string applicationKey, ushort port) : base(applicationKey, port)
        {
            // Loading plugins...
            PluginLoader = new PluginLoader<ServerPlugin>(PluginsLocation, (p) =>
            {
                p.Server = this;
                p.Commands = CommandProvider;
                p.Logger = Logger;
            }, Logger);

            Matchmaking = new ServerMatchmaking(this);

            RequestHandlers.Add(Requests.RequestMatchmaking, RequestMatchmaking);
            RequestHandlers.Add(Requests.RequestNewRoom, RequestNewRoom);
            RequestHandlers.Add(Requests.RequestJoinRandomRoom, RequestJoinRandomRoom);
            RequestHandlers.Add(Requests.RequestJoinNewRoom, RequestJoinNewRoom);
            RequestHandlers.Add(Requests.RequestJoinRoom, RequestJoinRoom);
            //RequestHandlers.Add(Identifiers.REQUEST_MODIFY_ROOM, RequestModifyRoom);
            RequestHandlers.Add(Requests.RequestCurrentRoom, RequestCurrentRoom);
            RequestHandlers.Add(Requests.RequestLeaveRoom, RequestLeaveRoom);
            RequestHandlers.Add(Requests.RequestKickFromRoom, RequestKickFromRoom);
            RequestHandlers.Add(Requests.RequestBroadcastInRoom, RequestBroadcastInRoom);
            RequestHandlers.Add(Requests.RequestSetClientInfo, RequestSetClientInfo);
            RequestHandlers.Add(Requests.RequestClientInfo, RequestClientInfo);
            RequestHandlers.Add(Requests.RequestClientIdentity, RequestClientIdentity);
            RequestHandlers.Add(Requests.RequestIncreaseAchievement, RequestIncreaseAchievement);
            RequestHandlers.Add(Requests.RequestClientLevel, RequestClientLevel);
            RequestHandlers.Add(Requests.RequestChangeClientRoomStatus, RequestChangeClientRoomStatus);
            RequestHandlers.Add(Requests.RequestChat, RequestChat);
            RequestHandlers.Add(Requests.RequestSetAvatar, RequestSetAvatar);

            Achievements = AchievementSystemLoader.LoadFromFile(AchievementFile);

            Achievement.OnGranted += (info) =>
            {
                BaseClientData<ClientIdentity> client;
                ConnectedClients.TryGetValue(info.GrantedToClient, out client);

                if (client != null)
                {
                    Send(client, Packet.SingleDataPacket(NetworkID, "_Achievement", info).ToBytes());

                    Logger.Log($"{ info.GrantedToClient } got the { info.Name } achievement, reward: { info.RewardLevels } levels!", LoggedImportance.Debug);
                }
            };

            // Finnaly activate the loaded plugins...
            PluginLoader.EnablePlugins();
        }

        public override void RegisterNetworkTypes()
        {
            StxNet.RegisterNetworkTypes();

            Bytifier.Include<BList<Room>>();
            Bytifier.Include<BConcurrentList<Room>>();
            Bytifier.Include<MatchmakingQuery>();
            Bytifier.Include<MatchmakingQueryResult>();
            Bytifier.Include<Room>();
            Bytifier.Include<RoomTemplate>();
            Bytifier.Include<ClientIdentity>();
            Bytifier.Include<ClientInfo>();
            Bytifier.Include<ChatEntry>();
            Bytifier.Include<AchievementGrantedInfo>();
            Bytifier.IncludeEnum<ClientStatus>();
            Bytifier.IncludeEnum<ClientRoomStatus>();
            Bytifier.IncludeEnum<GameState>();
            Bytifier.IncludeEnum<ChatSourceType>();
        }

        /// <summary>
        /// Invokes handlers for specific requests on this server.
        /// </summary>
        /// <param name="request">The request packet that needs a handler.</param>
        /// <param name="sender">The sender of this packet.</param>
        /// <returns>The fact that it found and invoked a handler.</returns>
        public override bool InvokeRequestHandlers(RequestPacket request, BaseClientData<ClientIdentity> sender)
        {
            if (!base.InvokeRequestHandlers(request, sender))
            {
                if (!sender.Identity.IsInAnyRoom || !RequestHandlersPerRoom.ContainsKey(sender.Identity.RoomID))
                    return false;

                ConcurrentDictionary<string, RequestDelegate<ClientIdentity>> requestHandlers = RequestHandlersPerRoom[sender.Identity.RoomID];

                if (requestHandlers == null || !requestHandlers.ContainsKey(request.RequestItemName))
                    return false;

                requestHandlers[request.RequestItemName].Invoke(request, sender);

                return true;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Creates a request handler that is only valid for the clients in a specific room.
        /// </summary>
        /// <param name="roomID">The ID of the room to create a handler for.</param>
        /// <param name="requiredKey">The required request packet key.</param>
        /// <param name="handler">The actual handler for the specified key.</param>
        public void CreateRoomRequestHandler(string roomID, string requiredKey, RequestDelegate<ClientIdentity> handler)
        {
            if (string.IsNullOrEmpty(roomID) || string.IsNullOrWhiteSpace(requiredKey) || handler == null)
                return;

            if (!RequestHandlersPerRoom.ContainsKey(roomID))
                RequestHandlersPerRoom.TryAdd(roomID, new ConcurrentDictionary<string, RequestDelegate<ClientIdentity>>());

            ConcurrentDictionary<string, RequestDelegate<ClientIdentity>> requestHandlers = RequestHandlersPerRoom[roomID];

            if (requestHandlers == null) // This should not be possible!
                return;

            if (!requestHandlers.ContainsKey(requiredKey))
                requestHandlers.TryAdd(requiredKey, handler);
            else
                requestHandlers[requiredKey] = handler;
        }

        /// <summary>
        /// Removes a room-specific request handler.
        /// </summary>
        /// <param name="roomID">The ID of the room to remove a handler from.</param>
        /// <param name="key">The key of the handler to remove.</param>
        public void RemoveRoomRequestHandler(string roomID, string key)
        {
            if (string.IsNullOrEmpty(roomID) || string.IsNullOrWhiteSpace(key) || !RequestHandlersPerRoom.ContainsKey(roomID))
                return;

            ConcurrentDictionary<string, RequestDelegate<ClientIdentity>> requestHandlers = RequestHandlersPerRoom[roomID];

            if (requestHandlers == null || !requestHandlers.ContainsKey(key))
                return;

            RequestDelegate<ClientIdentity> _;
            requestHandlers.TryRemove(key, out _);
        }

        /// <summary>
        /// Removes all the room-specific requests for a specified room.
        /// </summary>
        /// <param name="roomID">The ID of the room to remove all request handlers from.</param>
        public void RemoveAllRoomRequestHandlers(string roomID)
        {
            if (string.IsNullOrEmpty(roomID) || !RequestHandlersPerRoom.ContainsKey(roomID))
                return;

            ConcurrentDictionary<string, RequestDelegate<ClientIdentity>> _;
            RequestHandlersPerRoom.TryRemove(roomID, out _);
        }

        public override void Stop()
        {
            Logger.Log($"Disabling plugins...");
            PluginLoader.DisablePlugins();

            Logger.Log($"Saving achievements...");
            AchievementSystemLoader.SaveToFile();

            base.Stop();
        }

        public override BaseClientData<ClientIdentity> GetClientDataForAccepted(Socket accepted)
        {
            var cd = new ClientData(this, accepted);

            cd.MaxReceivesPerToken = MaxReceivesPerSecondPerClient;
            cd.MaxTimeoutSeconds = MaxTimeoutSeconds;
            cd.PingIntervalSeconds = PingIntervalSeconds;
            cd.TimedOutAction = new Action<BaseClientData<ClientIdentity>>((c) =>
            {
                DisconnectClient(c, DisconnectReason.TimedOut);
            });
            cd.Connected = true;

            return cd;
        }

        public override Packet GetAcknowledgmentPacket(BaseClientData<ClientIdentity> forClient, bool isFirstTimeConnecting)
        {
            Packet p = base.GetAcknowledgmentPacket(forClient, isFirstTimeConnecting);
            //p.Data.Add("StillInRoom", forClient.Identity.IsInRoom);
            return p;
        }

        public new ClientData GetConnectedClient(string clientID)
        {
            return (ClientData) base.GetConnectedClient(clientID);
        }

        #region Request Types

        private void RequestSetAvatar(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (!packet.Data.Requires<string>("NewAvatarUrl"))
                return;

            string newAvatarUrl = ((string)packet.Data["NewAvatarUrl"]).Trim();

            if (Uri.IsWellFormedUriString(newAvatarUrl, UriKind.Absolute))
            {
                sender.Identity.Info.AvatarUrl = newAvatarUrl;
                sender.Save();

                packet.ResponseOk();
            }
        }

        private void RequestMatchmaking(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            MatchmakingQuery query;

            if (!packet.Data.Requires<MatchmakingQuery>("Query"))
            {
                query = MatchmakingQuery.Default;
            }
            else
            {
                query = packet.Data.Get<MatchmakingQuery>("Query");
            }

            if (query != null)
            {
                packet.Respond(Matchmaking.GetMatches(query));
            }
        }

        private void RequestNewRoom(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (sender.Identity.IsInAnyRoom)
                return;

            if (packet.Data.Requires<RoomTemplate>("RoomTemplate"))
            {
                RoomTemplate rt = (RoomTemplate)packet.Data["RoomTemplate"];

                Room r = Matchmaking.LetCreateNewRoom((ClientData)sender, rt)?.Underlaying;

                if (r != null)
                    packet.Respond(r);
            }
        }

        private void RequestJoinRandomRoom(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (sender.Identity.IsInAnyRoom)
                return;

            if (packet.Data.Requires<MatchmakingQuery>("Query"))
            {
                MatchmakingQuery mq = (MatchmakingQuery)packet.Data["Query"];

                RoomTemplate fallback = packet.Data.Get<RoomTemplate>("RoomTemplate");

                ServerRoom r = null;

                if (fallback == null)
                {
                    r = Matchmaking.LetJoinRandomRoom((ClientData)sender, mq);
                }
                else
                {
                    r = Matchmaking.LetJoinRandomOrNewRoom((ClientData)sender, mq, fallback);
                }

                if (r != null)
                    packet.Respond(r.Underlaying);
            }
        }

        private void RequestJoinNewRoom(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (packet.Data.Requires<RoomTemplate>("RoomTemplate") && !sender.Identity.IsInAnyRoom)
            {
                RoomTemplate rt = (RoomTemplate)packet.Data["RoomTemplate"];

                ServerRoom r = Matchmaking.LetCreateNewRoom((ClientData)sender, rt);

                if (r == null)
                    return;

                r = Matchmaking.LetJoinRoom((ClientData)sender, r, rt.Password);

                if (r != null)
                    packet.Respond(r.Underlaying);
            }
        }

        private void RequestJoinRoom(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            string password = packet.Data.Get<string>("RoomPassword", null);

            if (packet.Data.Requires<string>("RoomID"))
            {
                string roomID = (string)packet.Data["RoomID"];

                if ((Matchmaking.GetRoom(roomID)?.Locked ?? false) && password == null)
                {
                    packet.ResponseRequires<string>("RoomPassword");

                    return;
                }

                ServerRoom r = Matchmaking.LetJoinRoom((ClientData)sender, roomID, password);

                if (r != null)
                    packet.Respond(r.Underlaying);
            }
            else if (packet.Data.Requires<string>("RoomCode"))
            {
                string roomCode = (string)packet.Data["RoomCode"];

                if ((Matchmaking.GetRoomWithCode(roomCode)?.Underlaying.Locked ?? false) && password == null)
                {
                    packet.ResponseRequires<string>("RoomPassword");

                    return;
                }

                ServerRoom r = Matchmaking.LetJoinRoomWithCode((ClientData)sender, roomCode, password);

                if (r != null)
                    packet.Respond(r.Underlaying);
            }
        }

        /*private void RequestModifyRoom(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (packet.Data.Requires<RoomTemplate>("RoomTemplate") && sender.ClientIdentity.IsInRoom)
            {
                RoomTemplate rt = (RoomTemplate)packet.Data["RoomTemplate"];

                string r = sender.ClientIdentity.RoomID;

                if (rt.IsValid())
                    if (Matchmaking.ModifyRoom(r, packet.SenderID, rt))
                        packet.ResponseOk();
            }
        }*/

        private void RequestCurrentRoom(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (sender.Identity.IsInAnyRoom)
            {
                Room r = Matchmaking.GetRoom(sender.Identity.RoomID);

                if (r == null || !r.Contains(sender.NetworkID))
                    return;

                packet.Respond(r);
            }
        }

        private void RequestLeaveRoom(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (!sender.Identity.IsInAnyRoom)
                return;

            Room r = Matchmaking.LetLeaveCurrentRoom((ClientData)sender)?.Underlaying;

            if (r != null)
                packet.Respond(r);
        }

        private void RequestKickFromRoom(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (!sender.Identity.IsInAnyRoom)
                return;

            ServerRoom room = Matchmaking.GetServerRoom(sender.Identity.RoomID);

            if (room == null)
                return;

            if (room.TryKick(sender.NetworkID, packet.Data.Get("ToKick", new string[0])))
                packet.ResponseOk();
        }

        private void RequestBroadcastInRoom(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (Matchmaking.LetBroadcastInRoom((ClientData)sender,
                packet.Data, 
                packet.Data.Get("ExcludeSender", false),
                packet.Data.Get("Receivers", new string[0])))
            {
                packet.ResponseOk();
            }
        }

        private void RequestSetClientInfo(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (packet.Data.Requires<ClientInfo>("ClientInfo"))
            {
                ClientInfo ci = (ClientInfo)packet.Data["ClientInfo"];

                if (ci.IsValid() && !ClientRegisterer<ClientIdentity>.IsNamePicked(ci.Name))
                {
                    sender.Identity.Info = ci;
                    sender.Save();

                    packet.ResponseOk();
                }
            }
        }

        private void RequestClientInfo(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (packet.Data.Requires<string>("ClientID"))
            {
                ClientData i = GetConnectedClient((string)packet.Data["ClientID"]);

                if (i != null)
                    packet.Respond(i.Identity.Info);
            }
            else
            {
                packet.Respond(sender.Identity.Info);
            }
        }

        private void RequestClientIdentity(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (packet.Data.Requires<string>("ClientID"))
            {
                ClientData i = GetConnectedClient((string)packet.Data["ClientID"]);

                if (i != null)
                    packet.Respond(i.Identity);
            }
            else
            {
                packet.Respond(sender.Identity);
            }
        }

        private void RequestIncreaseAchievement(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (packet.Data.Requires<string>("Achievement") && packet.Data.Requires<int>("Value"))
            {
                Achievement a = Achievements.GetFromName((string)packet.Data["Achievement"]);

                if (a == null)
                    return;

                int toAdd = (int)packet.Data["Value"];

                //if (OnAchievementIncrease?.Invoke((ClientData)sender, a, toAdd) ?? true)
                //{
                a.IncreaseValueFor(sender.NetworkID, toAdd);

                packet.Respond(a.GetValueFor(sender.NetworkID));
                //}
            }
        }

        private void RequestClientLevel(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (packet.Data.Requires<string>("ClientID"))
            {
                string client = (string)packet.Data["ClientID"];

                packet.Respond(Achievements.GetClientLevel(client));
            }
        }

        private void RequestChangeClientRoomStatus(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (packet.Data.Requires<ClientRoomStatus>("NewStatus") && sender.Identity.IsInAnyRoom)
            {
                ClientRoomStatus status = (ClientRoomStatus)packet.Data["NewStatus"];
                ClientRoomStatus rr = Matchmaking.LetChangeClientRoomStatus((ClientData)sender, status);

                if (rr != ClientRoomStatus.None)
                    packet.Respond(rr);
            }
        }

        private void RequestChat(RequestPacket packet, BaseClientData<ClientIdentity> sender)
        {
            if (!packet.Data.Requires<string>("ChatMessage") || !packet.Data.Requires<ChatSourceType>("ChatSourceType"))
                return;

            string message = (string)packet.Data["ChatMessage"];
            ChatSourceType type = (ChatSourceType)packet.Data["ChatSourceType"];
            ChatEntry chat = null;

            switch (type)
            {
                case ChatSourceType.Global:
                    if (!EnableGlobalChat)
                        return;
                    chat = new ChatEntry(sender.Identity, message, ChatSourceType.Global);
                    foreach (ClientData client in ConnectedClients.Values)
                        Send(client, Packet.SingleDataPacket(sender.NetworkID, "_Chat", chat).ToBytes());
                    break;

                case ChatSourceType.Personal:
                    if (!packet.Data.Requires<string>("ChatReceiverID"))
                        return;
                    string receiver = (string)packet.Data["ChatReceiverID"];
                    ClientData cd = GetConnectedClient(receiver);
                    if (cd == null)
                        return;
                    chat = new ChatEntry(sender.Identity, message, ChatSourceType.Personal);
                    byte[] p = Packet.SingleDataPacket(sender.NetworkID, "_Chat", chat).ToBytes();
                    Send(cd, p);
                    Send(sender, p);
                    break;

                case ChatSourceType.Room:
                    chat = new ChatEntry(sender.Identity, message, ChatSourceType.Room);
                    ServerRoom room = Matchmaking.GetRoomWhere(sender.Identity);
                    if (room == null)
                        return;
                    room.TryBroadcast(sender.NetworkID, new Hashtable()
                    {
                        ["_Chat"] = chat
                    }, false);
                    break;

                default:
                case ChatSourceType.Other:
                    break;
            }

            packet.ResponseOk();
        }

        #endregion
    }
}
