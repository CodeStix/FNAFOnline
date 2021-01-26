using Stx.Utilities;
using System;
using System.IO;
using System.Net;

namespace Stx.Net
{
    public class ConnectionStartInfo
    {
        public JsonConfig<LocalClientSettings> Config { get; private set; }

        public string Host { get; set; }
        public ushort Port { get; set; }
        public string ClientID { get; set; }
        public string AuthorizationToken { get; set; } = "5b1160ca-9d43-4fd0-aa99-61cb8f62d0d9";
        public string ApplicationName { get; set; } = StxNet.DefaultApplicationName;
        public string ApplicationVersion { get; set; } = StxNet.DefaultApplicationVersion;
        public string ApplicationKey { get; set; }
        public string ClientName { get; set; }
        public IPEndPoint EndPoint
        {
            get
            {
                var v = Dns.GetHostAddresses(Host);

                if (v.Length <= 0)
                    return new IPEndPoint(IPAddress.Any, 0);

                return new IPEndPoint(v[0], Port);
            }
        }
        /// <summary>
        /// When false, you have to call <see cref="BaseClient{TIdentity}.Connect()"/> manually when you want to connect to the server.
        /// </summary>
        public bool ConnectOnConstruct { get; set; } = true;
        public bool ConnectAsync { get; set; } = false;

        public const string DefaultLoadFile = @"CodeStix/Net/local.json";
        public const ushort DefaultPort = 1987;

        public ConnectionStartInfo(string hostColonPort, string applicationKey, bool loadFromDisk = true)
        {
            hostColonPort = hostColonPort.Trim();
            ApplicationKey = applicationKey;

            string h;
            ushort port = DefaultPort;

            if (hostColonPort.Contains(":"))
            {
                var s = hostColonPort.Split(':');
                h = s[0];
                port = ushort.Parse(s[1]);
            }
            else
            {
                h = hostColonPort;
            }

            Host = h;
            Port = port;

            if (loadFromDisk)
                LoadFromDisk(GetAppDataPath());
        }

        public ConnectionStartInfo(string host, ushort port, string applicationKey, string fileName)
        {
            Host = host;
            Port = port;
            ApplicationKey = applicationKey;

            if (!string.IsNullOrEmpty(fileName))
                LoadFromDisk(fileName);
        }

        public ConnectionStartInfo(string host, ushort port, string applicationKey, bool loadFromDisk = true)
        {
            Host = host;
            Port = port;
            ApplicationKey = applicationKey;

            if (loadFromDisk)
                LoadFromDisk(GetAppDataPath());
        }

        public void LoadFromDisk(string fileName)
        {
            this.Config = new JsonConfig<LocalClientSettings>(fileName);

            if (string.IsNullOrEmpty(Config.Settings.clientID))
            {
                Config.Settings.clientID = Guid.NewGuid().ToString();
                Config.Save();
            }

            if (string.IsNullOrEmpty(Config.Settings.authToken))
            {
                Config.Settings.authToken = Guid.NewGuid().ToString();
                Config.Save();
            }

            ClientID = Config.Settings.clientID;
            AuthorizationToken = Config.Settings.authToken;
            ClientName = Config.Settings.clientName;
        }

        public string GetAppDataPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DefaultLoadFile);
        }

        public bool IsValid()
        {
            if (!StringChecker.IsValidApp(ApplicationName, ApplicationVersion))
                return false;

            if (ApplicationKey.Length < 2)
                return false;

            if (!StringChecker.IsValidID(ClientID) || !StringChecker.IsValidID(AuthorizationToken))
                return false;

            return true;
        }

        public class LocalClientSettings
        {
            public string clientID = null;
            public string authToken = null;
            public string clientName = null;
        }
    }
}
