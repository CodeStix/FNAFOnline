using System.Collections;

namespace Stx.PluginArchitecture
{
    public interface IPlugin
    {
        string PluginName { get; }
        string PluginVersion { get; }
        bool Enabled { get; set; }
        void OnEnable();
        void OnDisable();
    }

    public interface IConfiguredPlugin : IPlugin
    {
        void Set<T>(string key, T value);
        T Get<T>(string key, T defaultValue = default(T));
        object Get(string key, object defaultValue = null);
        bool Contains<T>(string key);
        bool Contains(string key);
    }
}
