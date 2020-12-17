using Stx.Logging;
using Stx.Net.RoomBased;
using Stx.PluginArchitecture;
using Stx.Utilities;
using System;
using System.Collections;

namespace Stx.Net.ServerOnly.RoomBased
{
    public abstract class ServerPlugin : IConfiguredPlugin
    {
        public abstract string PluginName { get; }
        public abstract string PluginVersion { get; }
        public bool Enabled { get; set; } = false;

        public Server Server { get; set; }
        public ServerCommands<ClientIdentity> Commands { get; set; }
        public ILogger Logger { get; set; } = new VoidLogger();

        protected JsonConfig<Config> JsonConfig { get; set; }

        public ServerPlugin()
        {
            JsonConfig = new JsonConfig<Config>(GetConfigLocation());
        }

        public virtual void OnEnable()
        {
            Logger.Log($"Enabling plugin { PluginName }({ PluginVersion }) ...");
        }

        public virtual void OnDisable()
        {
            Logger.Log($"Disabling plugin { PluginName }({ PluginVersion }) ...");
        }

        public string GetConfigLocation()
        {
            return $@"{ Server.PluginsLocation + PluginName }\config.json";
        }

        public void Set<T>(string key, T value)
        {
            if (!JsonConfig.Settings.Settings.ContainsKey(key))
                JsonConfig.Settings.Settings.Add(key, value);
            else
                JsonConfig.Settings.Settings[key] = value;

            SaveConfig();
        }

        public T Get<T>(string key, T defaultValue = default(T))
        {
            if (!JsonConfig.Settings.Settings.ContainsKey(key))
                return defaultValue;
            else
                return (T)JsonConfig.Settings.Settings[key];
        }

        public object Get(string key, object defaultValue = null)
        {
            if (!JsonConfig.Settings.Settings.ContainsKey(key))
                return defaultValue;
            else
                return JsonConfig.Settings.Settings[key];
        }

        public bool Contains(string key)
        {
            return JsonConfig.Settings.Settings.ContainsKey(key);
        }

        public bool Contains<T>(string key)
        {
            return JsonConfig.Settings.Settings.ContainsKey(key) && JsonConfig.Settings.Settings[key].GetType() == typeof(T);
        }

        public object this[string key]
        {
            get
            {
                return Get<object>(key);
            }
            set
            {
                Set(key, value);
            }
        }

        public void SaveConfig()
        {
            JsonConfig.Save();
        }

        [Serializable]
        public class Config
        {
            public Hashtable Settings { get; set; } = new Hashtable();
        }
    }
}
