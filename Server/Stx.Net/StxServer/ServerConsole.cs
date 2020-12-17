using Stx.Logging;
using Stx.Net.ServerOnly;
using Stx.Net.ServerOnly.RoomBased;
using Stx.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Stx.Net.ServerProgram
{
    public static class ServerConsole
    {
        public static Server server;

        public static JsonConfig<ServerSettings> configFile;
        public static JsonConfig<BanList> banListFile;
        public static JsonConfig<VersionControl> versionFile;

        public const string ServerConfigFile = "server.json";
        public const string BannedClientsFile = "banList.json";
        public const string VersionsFile = "versions.json";

        private static void Main(string[] args)
        { 
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Stx.Net.ServerProgram");
            Console.WriteLine("\tVersion: " + StxServer.ServerStxVersion);
            Console.WriteLine("\tby Stijn Rogiest; 2018 (c)\n");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Starting...\n");

            //DebugHandler.AutoWriteToConsole = true;
            ThreadSafeData.MultiThreadOverride = false;
            StxNet.DefaultLogger = new ConsoleLogger();

            Console.WriteLine($"Reading { ServerConfigFile }...");
            configFile = new JsonConfig<ServerSettings>(ServerConfigFile);
            Console.WriteLine($"Reading { BannedClientsFile }...");
            banListFile = new JsonConfig<BanList>(BannedClientsFile);
            Console.WriteLine($"Reading { VersionsFile }...");
            versionFile = new JsonConfig<VersionControl>(VersionsFile);

            string appVersion = versionFile.Settings.currentVersion;
            string applicationKey = versionFile.Settings.GetCurrentVersion()?.applicationKey;
            string appName = versionFile.Settings.GetCurrentVersion()?.applicationName;
            string appDownload = versionFile.Settings.GetCurrentVersion()?.downloadLocation;
            ushort port = configFile.Settings.port;

            if (string.IsNullOrEmpty(applicationKey) || string.IsNullOrEmpty(appName))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Please configure the '{ VersionsFile }' file,");
                Console.WriteLine($"could not find the current version, delete the file to reset its properties.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();

                return;
            }

            versionFile.Settings.WriteToConsole();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();

            if (configFile.Settings.setupCompleted)
            {
                Console.WriteLine($"Server will accept client connections for {{{ applicationKey }}} { appName }({ appVersion }) on port { port }.\n");

                server = new Server(applicationKey, port);
                server.ApplicationName = appName;
                server.ApplicationVersion = appVersion;
                server.UpToDateApplicationUrl = appDownload;
                server.MaxConnections = configFile.Settings.maxConnections;
                server.PingIntervalSeconds = configFile.Settings.pingIntervalSeconds;
                server.MaxTimeoutSeconds = configFile.Settings.timeoutSeconds;
                server.NetworkID = configFile.Settings.serverID;
                server.BannedClients = new List<string>(banListFile.Settings.bannedClientIDs);
                server.BannedClientIPs = new List<string>(banListFile.Settings.bannedIPs);
                server.PluginLoader.Logger = StxNet.DefaultLogger;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Please configure the '{ ServerConfigFile }','{ VersionsFile }' files,");
                Console.WriteLine($"set 'setupCompleted' to true to start the server after modifying the properties.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();

                return;
            }

            Console.WriteLine();

            while (server.IsRunning)
            {
                string cmd = Console.ReadLine();

                if (!server.CommandProvider.Execute(cmd))
                {
                    server.CommandProvider.Output.Log("Command execution failed.", LoggedImportance.CriticalError);
                }
                else
                {
                    server.CommandProvider.Output.Log("Succeeded.", LoggedImportance.Successful);
                }
            }

            banListFile.Settings.bannedClientIDs = server.BannedClients.ToArray();
            banListFile.Settings.bannedIPs = server.BannedClientIPs.ToArray();
            banListFile.Save();

            Console.WriteLine("Server went offline, waiting 1 second...");
            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        public class ServerSettings
        {
            public ushort port = 37773;
            public int maxConnections = 50;
            public float pingIntervalSeconds = 10f;
            public float timeoutSeconds = 25f;
            public string serverID = "server";

            public bool setupCompleted = false;
        }

        public class BanList
        {
            public string[] bannedClientIDs = new string[] { };
            public string[] bannedIPs = new string[] { };
        }

        public class VersionControl
        {
            public string currentVersion = "1.0";
            public Version[] versions = new Version[] { new Version() };

            public Version GetCurrentVersion()
            {
                return versions.FirstOrDefault((v) => v.applicationVersion == currentVersion);
            }

            public void WriteToConsole()
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("----- Versions -----");
                foreach (Version v in versionFile.Settings.versions)
                {
                    bool current = currentVersion == v.applicationVersion;

                    if (current)
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    else
                        Console.ForegroundColor = ConsoleColor.DarkGray;

                    Console.WriteLine(v.ToString() + (current ? " (Currently used)" : null));
                }
            }

            public class Version
            {
                public string applicationKey = "ffffffff-ffff-ffff-ffff-ffffffffffff";
                public string applicationName = StxNet.DefaultApplicationName;
                public string applicationVersion = StxNet.DefaultApplicationVersion;
                public string downloadLocation = @"http://your.game.1.0.download.here";

                public Version() { }

                public Version(string appName, string appVersion, string downloadLocation)
                {
                    this.applicationName = appName;
                    this.applicationVersion = appVersion;
                    this.downloadLocation = downloadLocation;
                }

                public override string ToString()
                {
                    return $"{{{ applicationKey }}} { applicationName } ({ applicationVersion }): { downloadLocation }";
                }
            }
        }   

        
    }
}
