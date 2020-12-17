using Stx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Stx.Net.ServerOnly.RoomBased
{
    public delegate bool CommandDelegate(string[] args);

    public struct CommandInfo
    {
        public string Description { get; set; }
        public string[] Usages { get; set; }

        public CommandInfo(string description, params string[] usages)
        {
            this.Description = description;
            this.Usages = usages;
        }
    }

    public class ServerCommands<TIdentity> where TIdentity : NetworkIdentity, new()
    {
        public Dictionary<string, CommandDelegate> CommandHandlers { get; }
        public Dictionary<string, CommandInfo> CommandInfos { get; }
        public IBaseServer<TIdentity> Server { get; }
        public CommandDelegate HelpCommand { get; set; }
        public ILogger Output { get; set; } = StxNet.DefaultLogger;

        public ServerCommands(IBaseServer<TIdentity> server)
        {
            Server = server;

            CommandHandlers = new Dictionary<string, CommandDelegate>();
            CommandInfos = new Dictionary<string, CommandInfo>();

            CommandHandlers.Add("stop", CommandStop);
            CommandInfos.Add("stop", new CommandInfo("Stop the server.", "stop"));

            CommandHandlers.Add("announce", CommandAnnounce);
            CommandInfos.Add("announce", new CommandInfo("Send a message to all connected clients.", "announce <message>"));

            CommandHandlers.Add("list", CommandList);
            CommandInfos.Add("list", new CommandInfo("Get a list of all the connected clients.", "list"));

            CommandHandlers.Add("kick", CommandKick);
            CommandInfos.Add("kick", new CommandInfo("Kick a client from the server.", "kick <clientID>"));

            CommandHandlers.Add("ban", CommandBan);
            CommandInfos.Add("ban", new CommandInfo("Ban a clients id from the server.", "ban <clientID>"));

            CommandHandlers.Add("ban-ip", CommandBanIP);
            CommandInfos.Add("ban-ip", new CommandInfo("Ban a clients endpoint from the server.", "ban-ip <clientID|ip-address>"));

            CommandHandlers.Add("unban", CommandUnBan);
            CommandInfos.Add("unban", new CommandInfo("Lets a banned client back on this server.", "unban <clientID|ip-address>"));

            CommandHandlers.Add("banlist", CommandBanList);
            CommandInfos.Add("banlist", new CommandInfo("Displays a list of banned clients and ip-addresses", "banlist"));

            CommandHandlers.Add("stats", CommandStats);
            CommandInfos.Add("stats", new CommandInfo("Get statistics about this server.", "stats [socket|main]"));

            CommandHandlers.Add("identity", CommandIdentity);
            CommandInfos.Add("identity", new CommandInfo("Gathers information about offline or online clients, use 'identity list' to display all registered clients.", "identity list", "identity name <clientName>", "identity client <clientID>"));

            HelpCommand = (a) =>
            {
                Output.Log("&6List of available commands:");

                foreach (string k in CommandInfos.Keys)
                {
                    if (CommandInfos[k].Usages.Length == 0)
                    {
                        Output.Log($"&8\t- &e{ k } &7:&f { CommandInfos[k].Description }");
                    }
                    else if (CommandInfos[k].Usages.Length == 1)
                    {
                        Output.Log($"&8\t- &e{ CommandInfos[k].Usages[0] } &7:&f { CommandInfos[k].Description }");
                    }
                    else
                    {
                        Output.Log($"&8\t- &e{ k } ... &7:&f { CommandInfos[k].Description }");

                        for (int i = 0; i < CommandInfos[k].Usages.Length; i++)
                            Output.Log($"&8\t\t|-- &e{ CommandInfos[k].Usages[i] }");
                    }
                }

                return true;
            };
        }

        public bool Execute(string cmd)
        {
            string[] split = cmd.Split(' ');
            string[] args = split.Skip(1).ToArray();
            string n = split[0].ToLower();
            if (CommandHandlers.ContainsKey(n))
            {
                return CommandHandlers[n].Invoke(args);
            }
            else
            {
                HelpCommand?.Invoke(args);
                return true;
            }
        }

        #region Default Commands

        private bool CommandStats(string[] args)
        {
            if (args.Length == 0 || args[0] == "main")
            {
                Output.Log(ServerStats.GetStringRepresentation());
            }
            else
            {
                switch (args[0])
                {
                    case "socket":

                        var cd = Server.GetRandomConnectedClient();

                        if (cd == null)
                        {
                            Output.Log("Minimum 1 connection must be established.");
                            return false;
                        }

                        Output.Log("&aSocket&r properties for client &7" + cd.ToIdentifiedString());
                        Output.Log($"\t&e{ nameof(cd.Socket.Available) }&7: &r{ cd.Socket.Available.ToString() }");
                        Output.Log($"\t&e{ nameof(cd.Socket.Ttl) }&7: &r{ cd.Socket.Ttl.ToString() }");
                        Output.Log($"\t&e{ nameof(cd.Socket.SendBufferSize) }&7: &r{ cd.Socket.SendBufferSize.ToString() }");
                        Output.Log($"\t&e{ nameof(cd.Socket.ReceiveBufferSize) }&7: &r{ cd.Socket.ReceiveBufferSize.ToString() }");
                        Output.Log($"\t&e{ nameof(cd.Socket.ExclusiveAddressUse) }&7: &r{ cd.Socket.ExclusiveAddressUse.ToString() }");
                        Output.Log($"\t&e{ nameof(cd.Socket.Connected) }&7: &r{ cd.Socket.Connected.ToString() }");
                        Output.Log($"\t&e{ nameof(cd.Socket.Blocking) }&7: &r{ cd.Socket.Blocking.ToString() }");
                        Output.Log($"\t&e{ nameof(cd.Socket.ReceiveTimeout) }&7: &r{ cd.Socket.ReceiveTimeout.ToString() }");
                        Output.Log($"\t&e{ nameof(cd.Socket.SendTimeout) }&7: &r{ cd.Socket.SendTimeout.ToString() }");
                        break;

                    default:
                        return false;
                }
            }

            return true;
        }

        private bool CommandIdentity(string[] args)
        {
            int a = args.Length;

            if (a == 1 && args[0] == "list")
            {
                Output.Log($"&6List of all registered clients:");

                ClientRegisterer<TIdentity>.LoadRegisteredClients();
                foreach (TIdentity id in ClientRegisterer<TIdentity>.RegisteredClients)
                    Output.Log($"&8\t- &e{ id.Name }&7:&f { id.ToString().Replace("PreLobby", "&aPreLobby&f").Replace("InLobby", "&aInLobby&f").Replace("InGame", "&eInGame&f").Replace("Offline", "&cOffline&f") }");

                return true;
            }
            else if (a == 2 && args[0] == "name")
            {
                Output.Log($"&6List of all registered clients with name { args[1] }:");

                ClientRegisterer<TIdentity>.LoadRegisteredClients();
                foreach(var id in ClientRegisterer<TIdentity>.RegisteredClients.Where((e) => e.Name == args[1]))
                    Output.Log($"&8\t- &e{ id.Name }&7:&f { id.ToString() }");

                return true;
            }
            else if (a == 2 && args[0] == "id")
            {
                Output.Log($"&6Registered client with id { args[1] }:");

                TIdentity id = ClientRegisterer<TIdentity>.LoadIdentityFor(args[1]);
                if (id != null)
                    Output.Log($"&8\t- &e{ id.Name }&7:&f { id.ToString() }");

                return id != null;
            }
            else
            {
                Output.Log("identity list");
                Output.Log("identity name <clientName>");
                Output.Log("identity id <clientID>");

                return false;
            }
        }

        private bool CommandAnnounce(string[] args)
        {
            if (args.Length >= 1)
                Server.Announce(string.Join(" ", args));
            else
                Output.Log("announce <message>");

            return true;
        }

        private bool CommandStop(string[] args)
        {
            Server.Stop();

            return true;
        }

        private bool CommandList(string[] args)
        {
            Output.Log($"&6List of { Server.ConnectedCount } connected players:");

            foreach (var data in Server.GetConnectedClients())
                Output.Log($"&8\t- &e{ data.Name }&7:&f { data.ToIdentifiedString() }");

            return true;
        }

        private bool CommandKick(string[] args)
        {
            if (args.Length < 1)
                return false;

            var cd = Server.GetConnectedClient(args[0]);

            if (cd == null)
                return false;

            Server.DisconnectClient(cd, DisconnectReason.KickedByHost);

            Output.Info($"A client was kicked from the server: { cd.ToIdentifiedString() }");

            return true;
        }

        private bool CommandBan(string[] args)
        {
            if (args.Length < 1)
                return false;

            var cd = Server.GetConnectedClient(args[0]);

            if (cd != null)
            {
                Server.DisconnectClient(cd, DisconnectReason.KickedByHost);

                Output.Info($"A client was kicked from the server: { cd.ToIdentifiedString() }");
            }

            if (Server.BannedClients.Contains(args[0]))
            {
                Output.Log($"The client with id { args[0] } is already banned.");

                return true;
            }

            Server.BannedClients.Add(args[0]);

            Output.Success($"Any client with id { args[0] } is not allowed anymore on this server!");

            return true;
        }

        private bool CommandBanIP(string[] args)
        {
            if (args.Length < 1)
                return false;

            var cd = Server.GetConnectedClient(args[0]);
            string ipString = null;

            if (cd != null)
            {
                ipString = cd.RemoteIP.Address.ToString();

                Server.DisconnectClient(cd, DisconnectReason.KickedByHost);

                Output.Info($"A client was kicked from the server: { cd.ToIdentifiedString() }");
            }
            else
            {
                IPAddress ip;
                if (IPAddress.TryParse(args[0], out ip))
                    ipString = ip.ToString();
            }

            if (ipString == null)
            {
                Output.Error($"The client was not found or the given IP address was not valid.");

                return false;
            }

            if (Server.BannedClientIPs.Contains(ipString))
            {
                Output.Log($"The client with IP { ipString } is already banned.");

                return true;
            }

            Server.BannedClientIPs.Add(ipString);

            Output.Success($"Any client with IP address { ipString } is not allowed anymore on this server!");

            return true;
        }

        private bool CommandUnBan(string[] args)
        {
            if (args.Length < 1)
                return false;

            if (Server.BannedClients.Contains(args[0]))
            {
                Server.BannedClients.Remove(args[0]);

                Output.Info($"The client with id { args[0] } is allowed back on the server.");

                return true;
            }
            else if (Server.BannedClientIPs.Contains(args[0]))
            {
                Server.BannedClientIPs.Remove(args[0]);

                Output.Info($"The clients with IP { args[0] } is allowed back on the server.");

                return true;
            }
            else
            {
                Output.Error($"Could not remove { args[0] } from the ban-list, because it was not on it.");

                return false;
            }
        }

        private bool CommandBanList(string[] args)
        {
            if (Server.BannedClientIPs.Count == 0 && Server.BannedClients.Count == 0)
            {
                Output.Success("Great news, everyone is behaving. No banned clients were found.");

                return true;
            }

            Output.Log("&6List of banned clients:");

            foreach (string str in Server.BannedClients)
                Output.Log($"&8\t- &eBanned id&7:&f { str }");

            foreach (string str in Server.BannedClientIPs)
                Output.Log($"&8\t- &eBanned ip-address&7:&f { str }");

            return true;
        }

        #endregion

        #region Console Write Formatting

        /*/// <summary>
        /// Logger.Log formatted to console. Use the and-symbol to format.
        /// Black = 0,
        /// DarkBlue = 1,
        /// DarkGreen = 2,
        /// DarkCyan = 3,
        /// DarkRed = 4,
        /// DarkMagenta = 5,
        /// DarkYellow = 6,
        /// Gray = 7,
        /// DarkGray = 8,
        /// Blue = 9,
        /// Green = a,
        /// Cyan = b,
        /// Red = c,
        /// Magenta = d,
        /// Yellow = e,
        /// White = f
        /// </summary>
        /// <param name="str">The formatted string to display.</param>
        private static void WriteLine(string str) 
        {
            Write(str);
            Console.WriteLine();
        }

        /// <summary>
        /// Write formatted to console. Use the and-symbol to format.
        /// Black = 0,
        /// DarkBlue = 1,
        /// DarkGreen = 2,
        /// DarkCyan = 3,
        /// DarkRed = 4,
        /// DarkMagenta = 5,
        /// DarkYellow = 6,
        /// Gray = 7,
        /// DarkGray = 8,
        /// Blue = 9,
        /// Green = a,
        /// Cyan = b,
        /// Red = c,
        /// Magenta = d,
        /// Yellow = e,
        /// White = f
        /// </summary>
        /// <param name="str">The formatted string to display.</param>
        private static void Write(string str) 
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '&' && i != str.Length - 1)
                {
                    char c = str[i + 1];
                    int cc = -1;
                    if (c == 'a')
                        cc = 10;
                    if (c == 'b')
                        cc = 11;
                    if (c == 'c')
                        cc = 12;
                    if (c == 'd')
                        cc = 13;
                    if (c == 'e')
                        cc = 14;
                    if (c == 'f')
                        cc = 15;
                    if (c == 'r')
                        cc = 16;
                    if (cc == -1)
                        if (!int.TryParse(c.ToString(), out cc))
                            continue;
                    if (cc != 16)
                        Console.ForegroundColor = (ConsoleColor)cc;
                    else
                        Console.ResetColor();
                    i++;
                }
                else
                {
                    Console.Write(str[i]);
                }
            }
        }*/

        #endregion
    }
}