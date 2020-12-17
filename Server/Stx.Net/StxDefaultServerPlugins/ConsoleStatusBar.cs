using Stx.Net.RoomBased;
using Stx.Net.ServerOnly;
using Stx.Net.ServerOnly.RoomBased;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stx.Net.DefaultPlugin
{
    public class ConsoleStatusBar : ServerPlugin
    {
        public override string PluginName => "Console Status Bar";
        public override string PluginVersion => "1.0";

        public int RefreshRate { get; set; } = 4;

        private string statusBarTaskID;
        private IBaseClientData stalking = null;

        public override void OnEnable()
        {
            statusBarTaskID = Server.Scheduler.Repeat(() => UpdateStatusBar(), 1000 / RefreshRate).TaskID;

            Commands.CommandHandlers.Add("stalk", CommandStalk);
            Commands.CommandInfos.Add("stalk", new CommandInfo("Stalk a client by showing information in the title bar in real-time.", "stalk [clientID]"));

            base.OnEnable();
        }

        public override void OnDisable()
        {
            Server.Scheduler.StopRun(statusBarTaskID);

            base.OnDisable();
        }

        public void UpdateStatusBar()
        {
            Console.Title = GetStatus();
        }

        public string GetStatus()
        {
            if (stalking == null || !stalking.Connected)
            {
                if (stalking != null && !stalking.Connected)
                    stalking = null;

                return $"[Server Stats] "
                + $"{ Server.ApplicationName }({ Server.ApplicationVersion }) "
                + $"| { Server.ConnectedCount } / { Server.MaxConnections } players "
                + $"| { (GC.GetTotalMemory(false) / 1024) } kb "
                + $"| { Server.Matchmaking.RoomCount } rooms ";
            }
            else
            {
                return $"[Stalking { stalking.ToIdentifiedString() }] "
                + $"{ stalking.CurrentTokenReceives.ToString() }/{ stalking.MaxReceivesPerToken } ({ stalking.ReceiveToken.ToString() }) | "
                + $"{ stalking.LastReceiveInterval.ToString() }s response";
            }
        }

        private bool CommandStalk(string[] args)
        {
            if (args.Length != 1 || args[0] == "stop")
            {
                Logger.Log("Stalking was stopped.");

                stalking = null;

                return true;
            }
            else if (args.Length == 1)
            {
                Logger.Log($"Starting to stalk { args[0] }, look at the status bar.");

                stalking = Server.GetConnectedClient(args[0]);

                if (stalking == null)
                {
                    Logger.Log("The specified client was not found on the server.");

                    return false;
                }

                return true;
            }

            return false;
        }
    }
}
