using Stx.Net.ServerOnly;
using Stx.Net.ServerOnly.RoomBased;
using System.Linq;

namespace Stx.Net.DefaultPlugin
{
    public class Maintenance : ServerPlugin
    {
        public override string PluginName => "Maintenance Plugin";
        public override string PluginVersion => "1.0";

        private bool isActive = false;
        private string message;

        public override void OnEnable()
        {
            Commands.CommandInfos.Add("maintenance", new CommandInfo("Broadcast a maintenance message and shut the server down in the specified time.", "maintenance <minutesTilShutdown> <message>", "maintenance cancel"));
            Commands.CommandHandlers.Add("maintenance", (args) =>
            {
                if (args.Length == 1 && args[0].ToLower() == "cancel" && isActive)
                {
                    Server.Scheduler.StopRun(Shutdown);

                    isActive = false;

                    Logger.Log($"Operation was canceled!", Logging.LoggedImportance.Successful);

                    return true;
                }
                else if (args.Length > 1 && !isActive)
                {
                    int minutes;

                    if (!int.TryParse(args[0], out minutes))
                        return false;

                    message = string.Join(" ", args.Skip(1).ToArray());

                    Server.Scheduler.RunLater(Shutdown, minutes * 1000 * 60);
                    Server.Announce(message);

                    isActive = true;

                    Logger.Log($"Server will shut down in { minutes } minutes. Type 'maintenance cancel' to cancel this operation.");
                    Logger.Log($"Broadcasted message: { message }", Logging.LoggedImportance.Successful);

                    return true;
                }

                return false;
            });
        }

        public override void OnDisable()
        {
            Commands.CommandInfos.Remove("maintenance");
            Commands.CommandHandlers.Remove("maintenance");
        }

        private void Shutdown()
        {
            Server.Stop();
        }
    }
}
