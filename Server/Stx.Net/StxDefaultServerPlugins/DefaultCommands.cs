using Stx.Net.RoomBased;
using Stx.Net.ServerOnly.RoomBased;
using Stx.Utilities;
using System;

namespace Stx.Net.DefaultPlugin
{
    public class DefaultCommands : ServerPlugin
    {
        public override string PluginName => "Default Room Based Server Commands";
        public override string PluginVersion => "1.0";

        public override void OnEnable()
        {
            Commands.CommandHandlers.Add("room", CommandRoom);
            Commands.CommandInfos.Add("room", new CommandInfo("Modify or show info about a room.", "room [list]", "room <roomID> [destroy]", "room <roomID> [kick <clientID>]", "room <roomID> [gamestate <InGame|InLobby>]"));

            Commands.CommandHandlers.Add("reload", CommandReload);
            Commands.CommandInfos.Add("reload", new CommandInfo("Reload all the plugins.", "reload"));
        }

        public override void OnDisable()
        {
            Commands.CommandHandlers.Remove("room");
            Commands.CommandInfos.Remove("room");

            Commands.CommandHandlers.Remove("reload");
            Commands.CommandInfos.Remove("reload");
        }

        private bool CommandReload(string[] args)
        {
            Server.PluginLoader.Reload();

            return true;
        }

        private bool CommandRoom(string[] args)
        {
            if (args.Length <= 0 || args[0] == "list")
            {
                var rr = Server.Matchmaking.GetAllRooms();

                Logger.Log($"&6List of { rr.Count } rooms: ");

                foreach (ServerRoom r in rr)
                {
                    Logger.Log($"&8\t- &e{ r.Underlaying.Name }&7:&f {  r.ToString() }");
                }

                return true;
            }
            else if (args[0] == "create" && args.Length >= 2)
            {
                string name = args[1];

                RoomTemplate template = new RoomTemplate(name, 4);

                Room r = Server.Matchmaking.CreateNewRoom(template).Underlaying;
                r.OwnerClientID = "Server";

                return true;
            }
            else if (args[0] == "createrandom" && args.Length >= 2)
            {
                string countAsString = args[1];

                int count;
                if (int.TryParse(countAsString, out count))
                {
                    UniqueCodeGenerator ucg = new UniqueCodeGenerator();

                    for(int i = 0; i < count; i++)
                    {
                        RoomTemplate template = new RoomTemplate(ucg.GetNextCode(), 4);

                        Room r = Server.Matchmaking.CreateNewRoom(template).Underlaying;
                        r.OwnerClientID = "Server";
                    }
                }

                return true;
            }

            if (args.Length >= 1)
            {
                ServerRoom r = Server.Matchmaking.GetServerRoom(args[0]);

                if (r == null)
                {
                    Logger.Log($"The specified room was not found.", Logging.LoggedImportance.Warning);
                    return false;
                }

                if (args.Length == 2 && args[1] == "destroy")
                {
                    return Server.Matchmaking.DestroyRoom(r);
                }
                else if (args.Length == 3 && args[1] == "kick")
                {
                    r.KickAsServer(args[2].Split(','));//Server.Matchmaking.KickFromRoom(r.ID, Server.NetworkID, args[2].Split(','));
                    return true;
                }
                else if (args.Length == 3 && args[1] == "gamestate")
                {
                    GameState gs;
                    if (!Enum.TryParse(args[2], out gs))
                        return false;

                    return r.TryChangeGameState(gs);//Server.Matchmaking.ChangeGameState(r.ID, gs);
                }
                else
                {
                    Logger.Log($"-------- Information about room { r.ID } --------");
                    Logger.Log(nameof(r.Underlaying.ConnectedClients) + ": " + string.Join(",", r.Underlaying.ConnectedClients.ToArray()));
                    Logger.Log(nameof(r.Underlaying.Tags) + ": " + string.Join(",", r.Underlaying.Tags.ToArray()));
                    Logger.Log(nameof(r.Underlaying.CreationTime) + ": " + r.Underlaying.CreationTime);
                    Logger.Log(nameof(r.Underlaying.EnoughPlayers) + ": " + r.Underlaying.EnoughPlayers);
                    Logger.Log(nameof(r.Underlaying.ID) + ": " + r.Underlaying.ID);
                    Logger.Log(nameof(r.Underlaying.IsFull) + ": " + r.Underlaying.IsFull);
                    Logger.Log(nameof(r.Underlaying.OwnerClientID) + ": " + r.Underlaying.OwnerClientID);
                    Logger.Log(nameof(r.Underlaying.OwnerDisplayName) + ": " + r.Underlaying.OwnerDisplayName);
                    Logger.Log(nameof(r.Underlaying.PlayerCount) + ": " + r.Underlaying.PlayerCount);
                    Logger.Log(nameof(r.Underlaying.RequiredPlayerCount) + ": " + r.Underlaying.RequiredPlayerCount);
                    Logger.Log(nameof(r.Underlaying.Name) + ": " + r.Underlaying.Name);
                    Logger.Log(nameof(r.Underlaying.MaxPlayers) + ": " + r.Underlaying.MaxPlayers);
                    Logger.Log(nameof(r.Underlaying.Locked) + ": " + r.Underlaying.Locked);
                    Logger.Log(nameof(r.HashedRoomPassword) + ": " + r.HashedRoomPassword);
                    Logger.Log(nameof(r.Underlaying.TaggedClients) + ": ");
                    foreach(string client in r.Underlaying.TaggedClients.Keys)
                        Logger.Log("\t" + client + ": " + r.Underlaying.TaggedClients[client]);
                    Logger.Log(nameof(r.Application) + ": " + (r.Application?.ToString() ?? "<No bound object>"));
                }

                return true;
            }

            return false;
        }

    }
}
