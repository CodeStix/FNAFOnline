namespace Stx.Net
{
    /// <summary>
    /// Request item name identifiers.
    /// </summary>
    public static class Requests
    {
        #region Default Request Identifiers

        public const string RequestPing = "Ping";
        public const string RequestSetName = "SetName";
        public const string RequestDisconnect = "Disconnect";

        #endregion

        #region Altered Request Identifiers

        public const string RequestClientIdentity = "ClientIdentity";
        public const string RequestMatchmaking = "Matchmaking";
        public const string RequestNewRoom = "NewRoom";
        public const string RequestJoinNewRoom = "JoinNewRoom";
        public const string RequestJoinRandomRoom = "JoinRandomRoom";
        public const string RequestJoinRoom = "JoinRoom";
        //public const string RequestModifyRoom = "ModifyRoom";
        public const string RequestCurrentRoom = "CurrentRoom";
        public const string RequestLeaveRoom = "LeaveRoom";
        public const string RequestKickFromRoom = "KickFromRoom";
        public const string RequestBroadcastInRoom = "BroadcastInRoom";
        public const string RequestSetClientInfo = "SetClientInfo";
        public const string RequestClientInfo = "ClientInfo";
        public const string RequestIncreaseAchievement = "IncreaseAchievement";
        public const string RequestClientLevel = "ClientLevel";
        public const string RequestChangeClientRoomStatus = "ChangeClientRoomStatus";
        public const string RequestChat = "Chat";
        public const string RequestSetAvatar = "SetAvatar";

        #endregion
    }
}
