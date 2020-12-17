using Stx.Logging;
using Stx.Serialization;
using Stx.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stx.Net
{
    public static class ClientRegisterer<TIdentity> where TIdentity : NetworkIdentity, new()
    {
        public const string DataSaveLocation = @".\ClientData\";
        public const string DataSaveExtension = ".bin";
        public const string AuthSaveExtension = ".auth";

        public static HashSet<string> PickedLowClientNames { get; set; }
        public static HashSet<TIdentity> RegisteredClients { get; private set; }

        public static ILogger Logger = StxNet.DefaultLogger;

        public static string GetFullDataSavePath(string forClientID)
        {
            return Path.Combine(DataSaveLocation, forClientID) + DataSaveExtension;
        }

        public static string GetFullAuthSavePath(string forClientID)
        {
            return Path.Combine(DataSaveLocation, forClientID) + AuthSaveExtension;
        }

        public static bool IdentityExistsFor(string clientID)
        {
            try
            {
                return File.Exists(GetFullDataSavePath(clientID));
            }
            catch (Exception e)
            {
                Logger.LogException(e, "Could not check ClientIdentity!");

                return false;
            }
        }

        public static TIdentity LoadIdentityFor(string clientID)
        {
            string file = GetFullDataSavePath(clientID);

            TIdentity ti = LoadIdentityFromFile(file);

            if (ti == default(TIdentity))
            {
                ti = new TIdentity();
                ti.SetClientID(clientID);
            }

            return ti;
        }

        public static bool SaveIdentityFor(TIdentity identity)
        {
            if (identity.NetworkID == NetworkIdentity.UnknownID || identity == default(TIdentity))
                return false;

            string file = GetFullDataSavePath(identity.NetworkID);

            try
            {
                Bytifile.ToFile(file, identity);

                return true;
            }
            catch (Exception e)
            {
                Logger.LogException(e, "Could not save ClientIdentity!");

                return false;
            }
        }

        public static TIdentity LoadIdentityFromFile(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    return Bytifile.FromFile<TIdentity>(file); ;
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e, "Could not load ClientIdentity!");
            }

            return default(TIdentity);
        }

        /*[Obsolete]
        private static TIdentity LoadIdentityFromDisk(string file)
        {
            if (!File.Exists(file))
                return default(TIdentity);

            try
            {
                return Bytifile.FromFile<TIdentity>(file);
            }
            catch (Exception e)
            {
                Logger.LogException(e, "Could not load ClientIdentity");

                return default(TIdentity);
            }
        }*/

        /*public static TIdentity LoadRegisteredClient(string clientID)
        {
            string file = DATA_SAVE_LOCATION + clientID + DATA_SAVE_EXTENSION;

            if (!File.Exists(file))
                return null;

            return LoadIdentityFromDisk(file);
        }*/

        public static void LoadRegisteredClients()
        {
            RegisteredClients = new HashSet<TIdentity>();
            PickedLowClientNames = new HashSet<string>();

            try
            {
                foreach (string file in Directory.GetFiles(DataSaveLocation))
                {
                    if (file.EndsWith(DataSaveExtension))
                    {
                        var t = LoadIdentityFromFile(file);

                        if (t == null)
                            continue;

                        RegisteredClients.Add(t);
                        PickedLowClientNames.Add(t.Name?.ToLower());
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e, "Could not load registered clients!");
            }
        }

        public static bool IsNamePicked(string name)
        {
            return PickedLowClientNames.Contains(name.ToLower());
        }

        public static bool TryGetClientID(string clientName, out string clientID)
        {
            TIdentity i = RegisteredClients.FirstOrDefault((e) => e.Name.EqualsIgnoreCase(clientName));

            if (i == null)
            {
                clientID = null;

                return false;
            }
            else
            {
                clientID = i.NetworkID;

                return true;
            }
        }
    }
}
