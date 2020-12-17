using Stx.Logging;
using Stx.Net.ServerOnly.RoomBased;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stx.Net.DefaultPlugin
{
    public class TestRoomApplication : ServerPlugin
    {
        public override string PluginName => "Testing Room Application";
        public override string PluginVersion => "1.0";

        public override void OnEnable()
        {
            //Server.Matchmaking.OnNewRoom += Matchmaking_OnNewRoom;
            //Server.Matchmaking.OnRoomDestroyed += Matchmaking_OnRoomDestroyed;
        }

        public override void OnDisable()
        {
            //Server.Matchmaking.OnNewRoom -= Matchmaking_OnNewRoom;
            //Server.Matchmaking.OnRoomDestroyed -= Matchmaking_OnRoomDestroyed;
        }

        private void Matchmaking_OnRoomDestroyed(ServerRoom room)
        {
            Logger.Debug("Room was destroyed: " + room.ToString());
        }

        private void Matchmaking_OnNewRoom(ServerRoom room, string creator)
        {
            Logger.Debug($"Room was created by { creator }: " + room.Underlaying.Name);

            room.Application = new TestApp();
        }
    }

    public class TestApp : RoomApplication
    {
        public override void OnInitialize()
        {
            Logger.Debug("Room is initializing...");

            RegisterHandler("Move", (packet, sender) =>
            {
                Logger.Info("Received a move from " + sender.ToIdentifiedString());

                packet.Respond("dit is een response.");
            });
        }

        public override void OnTerminate()
        {
            Logger.Debug("Room is terminating...");

            UnRegisterAllHandlers();
        }

        public override void OnCanStart()
        {
            Logger.Debug("Room can start!");
        }

        public override void OnStart()
        {
            Logger.Debug("Room has started!");
        }

        public override void OnEnd()
        {
            Logger.Debug("Room has ended!");
        }

        public override void OnJoined(ClientData whoJoined)
        {
            Logger.Debug("Client joined room: " + whoJoined.ToIdentifiedString());
        }

        public override void OnLeft(ClientData whoLeft)
        {
            Logger.Debug("Client left room: " + whoLeft.ToIdentifiedString());
        }

        public override void OnPromoted(string newOwnerID)
        {
            Logger.Debug("Room got new owner: " + newOwnerID);
        }
    }
}
