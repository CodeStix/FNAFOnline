using Stx.Net.ServerOnly;
using Stx.Net.ServerOnly.RoomBased;
using Stx.Net.VoiceBytes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FNAFOnline.Server
{
    public class VoiceChat_Server : ServerPlugin
    {
        public override string PluginName { get; } = "VoiceChat";
        public override string PluginVersion { get; } = "1.0";

        public ushort Port { get; set; } = 11987;
        public VoiceServer VoiceServer { get; private set; }
        public Dictionary<string, VoiceRoom> VoiceRooms { get; } = new Dictionary<string, VoiceRoom>();

        public override void OnEnable()
        {
            if (JsonConfig.IsFirstLoad)
            {
                Logger.Log("&aFirst time load.");

                Set("Port", 11987);
            }

            Port = (ushort)Convert.ToInt16(Get("Port"));

            Logger.Log("&eVoiceChat plugin enabled.", Stx.Logging.LoggedImportance.Successful);

            ServerRoom.OnJoinRoom += Server_OnClientJoinRoom;
            ServerRoom.OnLeaveRoom += Server_OnClientLeaveRoom;

            VoiceServer = new VoiceServer(Port);
            VoiceServer.EnableGlobalRoom = true;
        }

        public override void OnDisable()
        {
            VoiceServer.Dispose();
        }

        private void Server_OnClientJoinRoom(ClientData arg, ServerRoom arg2)
        {
            if (!VoiceRooms.ContainsKey(arg2.ID))
                VoiceRooms.Add(arg2.ID, new VoiceRoom());


            VoiceRoom s = VoiceRooms[arg2.ID];

            Logger.Log($"&e{ arg.Identity.DisplayName } has joined voice room { s.RoomID }.");

            if (!s.AllowedClients.Contains(arg.RemoteIP))
                s.AllowedClients.Add(arg.RemoteIP);
        }

        private void Server_OnClientLeaveRoom(ClientData arg, ServerRoom arg2)
        {
            if (!VoiceRooms.ContainsKey(arg2.ID))
                return;

            VoiceRoom s = VoiceRooms[arg2.ID];

            Logger.Log($"&e{ arg.Identity.DisplayName } has left voice room { s.RoomID }.");

            if (s.AllowedClients.Contains(arg.RemoteIP))
                s.AllowedClients.Remove(arg.RemoteIP);

            if (s.AllowedClients.Count <= 0)
                VoiceRooms.Remove(arg2.ID);
        }
    }
}