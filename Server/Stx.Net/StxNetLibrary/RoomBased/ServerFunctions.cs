using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;
using Stx.Logging;
using Stx.Net;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;

namespace Stx.Net.RoomBased
{
    public class ServerFunctions : ServerRequester
    {
        public ServerFunctions(IServerConnected client) : base(client)
        { }

        public ILogger Logger { get; set; } = StxNet.DefaultLogger;

        public void GetMatchmakingAsync(MatchmakingQuery query, ServerResponse<MatchmakingQueryResult> matchmakingResponse)
        {
            RequestAsync<MatchmakingQueryResult>(Requests.RequestMatchmaking, matchmakingResponse, SingleData("Query", query));
        }

        public Task<MatchmakingQueryResult> GetMatchmaking(MatchmakingQuery query)
        {
            return Request<MatchmakingQueryResult>(Requests.RequestMatchmaking, SingleData("Query", query));
        }

        public void JoinNewRoomAsync(RoomTemplate newRoomTemplate, ServerResponse<Room> newRoomResponse)
        {
            RequestAsync<Room>(Requests.RequestJoinNewRoom, newRoomResponse, SingleData("RoomTemplate", newRoomTemplate));
        }

        public Task<Room> JoinNewRoom(RoomTemplate newRoomTemplate)
        {
            return Request<Room>(Requests.RequestJoinNewRoom, SingleData("RoomTemplate", newRoomTemplate));
        }

        public void GetNewRoomAsync(RoomTemplate newRoomTemplate, ServerResponse<Room> newRoomResponse)
        {
            RequestAsync<Room>(Requests.RequestNewRoom, newRoomResponse, SingleData("RoomTemplate", newRoomTemplate));
        }

        public Task<Room> GetNewRoom(RoomTemplate newRoomTemplate)
        {
            return Request<Room>(Requests.RequestNewRoom, SingleData("RoomTemplate", newRoomTemplate));
        }

        public void JoinRoomAsync(string roomID, ServerResponse<Room> joinedRoomResponse, string roomPassword = null)
        {
            Hashtable t = new Hashtable();
            t.Add("RoomID", roomID);
            if (roomPassword != null)
                t.Add("RoomPassword", roomPassword);

            RequestAsync<Room>(Requests.RequestJoinRoom, joinedRoomResponse, t);
        }

        public Task<Room> JoinRoom(string roomID, string roomPassword = null)
        {
            Hashtable t = new Hashtable();
            t.Add("RoomID", roomID);
            if (roomPassword != null)
                t.Add("RoomPassword", roomPassword);

            return Request<Room>(Requests.RequestJoinRoom, t);
        }

        public void JoinRoomWithCodeAsync(string roomCode, ServerResponse<Room> joinedRoomResponse, string roomPassword = null)
        {
            Hashtable t = new Hashtable();
            t.Add("RoomCode", roomCode);
            if (roomPassword != null)
                t.Add("RoomPassword", roomPassword);

            RequestAsync<Room>(Requests.RequestJoinRoom, joinedRoomResponse, t);
        }

        public Task<Room> JoinRoomWithCode(string roomCode, string roomPassword = null)
        {
            Hashtable t = new Hashtable();
            t.Add("RoomCode", roomCode);
            if (roomPassword != null)
                t.Add("RoomPassword", roomPassword);

            return Request<Room>(Requests.RequestJoinRoom, t);
        }

        public void JoinRandomRoomAsync(MatchmakingQuery mustMatch, ServerResponse<Room> joinedRoomResponse, RoomTemplate fallbackNewRoomTemplate = null)
        {
            Hashtable t = new Hashtable();
            t.Add("Query", mustMatch);
            if (fallbackNewRoomTemplate != null)
                t.Add("RoomTemplate", fallbackNewRoomTemplate);

            RequestAsync<Room>(Requests.RequestJoinRandomRoom, joinedRoomResponse, t);
        }

        public Task<Room> JoinRandomRoom(MatchmakingQuery mustMatch, RoomTemplate fallbackNewRoomTemplate = null)
        {
            Hashtable t = new Hashtable();
            t.Add("Query", mustMatch);
            if (fallbackNewRoomTemplate != null)
                t.Add("RoomTemplate", fallbackNewRoomTemplate);

            return Request<Room>(Requests.RequestJoinRandomRoom, t);
        }

        public void LeaveRoomAsync(ServerResponse<Room> leftRoomResponse)
        {
            RequestAsync<Room>(Requests.RequestLeaveRoom, leftRoomResponse);
        }

        public Task<Room> LeaveRoom()
        {
            return Request<Room>(Requests.RequestLeaveRoom);
        }

        public void KickFromRoomAsync(ServerResponse<object> kickedResponse, params string[] clientsToKick)
        {
            RequestAsync<object>(Requests.RequestKickFromRoom, kickedResponse, SingleData("ToKick", clientsToKick));
        }

        public Task KickFromRoom(ServerResponse<object> kickedResponse, params string[] clientsToKick)
        {
            return Request<object>(Requests.RequestKickFromRoom, SingleData("ToKick", clientsToKick));
        }

        public void BroadcastInRoomAsync(Hashtable dataToBroadcast, ServerResponse<object> broadcastedResponse, bool excludeSender = true, params string[] receivers)
        {
            Hashtable t = new Hashtable(dataToBroadcast);
            if (receivers != null && receivers?.Length > 0)
                t.Add("Receivers", receivers);
            t.Add("ExcludeSender", excludeSender);

            RequestAsync<object>(Requests.RequestBroadcastInRoom, broadcastedResponse, t);
        }

        public Task<object> BroadcastInRoom(Hashtable dataToBroadcast, bool excludeSender = true, params string[] receivers)
        {
            Hashtable t = new Hashtable(dataToBroadcast);
            if (receivers != null && receivers?.Length > 0)
                t.Add("Receivers", receivers);
            t.Add("ExcludeSender", excludeSender);

            return Request<object>(Requests.RequestBroadcastInRoom, t);
        }

        public void GetClientIdentityAsync(ServerResponse<ClientIdentity> clientInfoResponse)
        {
            RequestAsync<ClientIdentity>(Requests.RequestClientIdentity, clientInfoResponse);
        }

        public void GetClientIdentityAsync(string clientID, ServerResponse<ClientIdentity> clientInfoResponse)
        {
            RequestAsync<ClientIdentity>(Requests.RequestClientIdentity, clientInfoResponse, SingleData("ClientID", clientID));
        }

        public Task<ClientIdentity> GetClientIdentity()
        {
            return Request<ClientIdentity>(Requests.RequestClientIdentity);
        }

        public Task<ClientIdentity> GetClientIdentity(string clientID)
        {
            return Request<ClientIdentity>(Requests.RequestClientIdentity, SingleData("ClientID", clientID));
        }

        public void SetClientInfoAsync(ClientInfo clientInfo, ServerResponse<ClientInfo> infoSetResponse)
        {
            RequestAsync<ClientInfo>(Requests.RequestSetClientInfo, infoSetResponse, SingleData("ClientInfo", clientInfo));
        }

        public Task<ClientInfo> SetClientInfo(ClientInfo clientInfo)
        {
            return Request<ClientInfo>(Requests.RequestSetClientInfo, SingleData("ClientInfo", clientInfo));
        }

        public void GetClientInfoAsync(ServerResponse<ClientInfo> infoResponse)
        {
            RequestAsync<ClientInfo>(Requests.RequestClientInfo, infoResponse);
        }

        public void GetClientInfoAsync(string clientID, ServerResponse<ClientInfo> infoResponse)
        {
            RequestAsync<ClientInfo>(Requests.RequestClientInfo, infoResponse, SingleData("ClientID", clientID));
        }

        public Task<ClientInfo> GetClientInfo()
        {
            return Request<ClientInfo>(Requests.RequestClientInfo);
        }

        public Task<ClientInfo> GetClientInfo(string clientID)
        {
            return Request<ClientInfo>(Requests.RequestClientInfo, SingleData("ClientID", clientID));
        }

        /*public void ModifyRoomAsync(RoomTemplate newTemplate, ServerResponse<RoomTemplate> modifiedResponse)
        {
            RequestAsync<RoomTemplate>(Identifiers.REQUEST_MODIFY_ROOM, modifiedResponse, SingleData("RoomTemplate", newTemplate));
        }

        public Task<RoomTemplate> ModifyRoom(RoomTemplate newTemplate)
        {
            return Request<RoomTemplate>(Identifiers.REQUEST_MODIFY_ROOM, SingleData("RoomTemplate", newTemplate));
        }*/

        public void GetCurrentRoomAsync(ServerResponse<Room> yourRoomResponse)
        {
            RequestAsync<Room>(Requests.RequestCurrentRoom, yourRoomResponse);
        }

        public Task<Room> GetCurrentRoom()
        {
            return Request<Room>(Requests.RequestCurrentRoom);
        }

        public void IncreaseAchievementAsync(string achievementName, int toAdd, ServerResponse<int> responseNewValue)
        {
            Hashtable t = new Hashtable();
            t.Add("Achievement", achievementName);
            t.Add("Value", toAdd);

            RequestAsync<int>(Requests.RequestIncreaseAchievement, responseNewValue, t);
        }

        public Task<int> IncreaseAchievement(string achievementName, int toAdd)
        {
            Hashtable t = new Hashtable();
            t.Add("Achievement", achievementName);
            t.Add("Value", toAdd);

            return Request<int>(Requests.RequestIncreaseAchievement, t);
        }

        public void GetClientLevelAsync(string clientID, ServerResponse<int> responseLevel)
        {
            RequestAsync<int>(Requests.RequestClientLevel, responseLevel, SingleData("ClientID", clientID));
        }

        public Task<int> GetClientLevel(string clientID)
        {
            return Request<int>(Requests.RequestClientLevel, SingleData("ClientID", clientID));
        }

        public void ChangeInRoomStatusAsync(ClientRoomStatus newStatus, ServerResponse<ClientRoomStatus> responseStatus)
        {
            RequestAsync<ClientRoomStatus>(Requests.RequestChangeClientRoomStatus, responseStatus, SingleData("NewStatus", newStatus));
        }

        public Task<ClientRoomStatus> ChangeInRoomStatus(ClientRoomStatus newStatus)
        {
            return Request<ClientRoomStatus>(Requests.RequestChangeClientRoomStatus, SingleData("NewStatus", newStatus));
        }

        public void ChatGloballyAsync(string message, ServerResponse chattedResponse)
        {
            Hashtable t = new Hashtable();
            t.Add("ChatMessage", message);
            t.Add("ChatSourceType", ChatSourceType.Global);

            RequestAsync(Requests.RequestChat, chattedResponse, t);
        }

        public void ChatInRoomAsync(string message, ServerResponse chattedResponse)
        {
            Hashtable t = new Hashtable();
            t.Add("ChatMessage", message);
            t.Add("ChatSourceType", ChatSourceType.Room);

            RequestAsync(Requests.RequestChat, chattedResponse, t);
        }

        public void ChatPersonalAsync(string message, string receiverID, ServerResponse chattedResponse)
        {
            Hashtable t = new Hashtable();
            t.Add("ChatMessage", message);
            t.Add("ChatReceiverID", receiverID);
            t.Add("ChatSourceType", ChatSourceType.Personal);

            RequestAsync(Requests.RequestChat, chattedResponse, t);
        }

        public void Disconnect()
        {
            RequestAsync(Requests.RequestDisconnect);
        }

        public void SetName(string newName, ServerResponse nameSetResponse)
        {
            Hashtable t = new Hashtable();
            t.Add("NewName", newName);

            RequestAsync(Requests.RequestSetName, nameSetResponse, t);
        }

        public void SetAvatarUrl(string urlToAvatar, ServerResponse avatarSetResponse)
        {
            Hashtable t = new Hashtable();
            t.Add("NewAvatarUrl", urlToAvatar);

            RequestAsync(Requests.RequestSetAvatar, avatarSetResponse, t);
        }

        public string SetAndUploadAvatar(string localFile, ServerResponse uploadedAndSetResponse)
        {
            if (string.IsNullOrEmpty(StxNet.ImgurApplicationClientId) || string.IsNullOrEmpty(StxNet.ImgurApplicationClientSecret))
            {
                Logger.CriticalWarning("Please configure the Imgur application settings. See StxNet.ImgurApplication");

                return null;
            }

            try
            {
                ImgurClient client = new ImgurClient(StxNet.ImgurApplicationClientId, StxNet.ImgurApplicationClientSecret);
                ImageEndpoint endpoint = new ImageEndpoint(client);

                IImage image;
                using (var fs = new FileStream(localFile, FileMode.Open))
                {
                    image = endpoint.UploadImageStreamAsync(fs).GetAwaiter().GetResult();
                }

                SetAvatarUrl(image.Link, uploadedAndSetResponse);

                return image.Link;
            }
            catch (ImgurException ex)
            {
                Logger.LogException(ex, "Could not upload avatar to Imgur.");

                return null;
            }
        }
    }
}
