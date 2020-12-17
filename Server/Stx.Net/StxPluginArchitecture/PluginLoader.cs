using Stx.Logging;
using Stx.Utilities.ErrorHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Stx.PluginArchitecture
{
    public class PluginLoader<T> where T : IPlugin
    {
        public ICollection<T> LoadedPlugins { get; private set; }
        public string PluginDirectory { get; private set; }
        public PreparePluginDelegate PluginPrepare { get; }

        public delegate void PreparePluginDelegate(T plugin);

        /// <summary>
        /// Loads plugins from a specified directory on disk. Only accepts *.dll plugins. 
        /// To create a plugin, first create a 'plugin structure' by inheriting <see cref="IPlugin"/> in your 'plugin structure' class. Then use the class as the PluginLoader's generic type.
        /// </summary>
        /// <param name="pluginDirectoryPath">The directory path to load compatible plugins from. <see cref="IPlugin"/></param>
        /// <param name="preparePlugin">The delegate to setup your plugin class. (ex. set some variables you will need in your plugin)</param>
        /// <param name="debugToConsole">Debug the progress to the console.</param>
        public PluginLoader(string pluginDirectoryPath, PreparePluginDelegate preparePlugin = null, ILogger logger = null)
        {
            this.PluginDirectory = pluginDirectoryPath;
            this.PluginPrepare = preparePlugin;

            if (logger != null)
                Logger = logger;

            Load();
        }

        public ILogger Logger { get; set; } = new VoidLogger();

        private void Load()
        {
            try
            {
                LoadedPlugins = new List<T>();

                Logger.Log("Loading plugins...");

                string[] dllFileNames = null;
                if (!Directory.Exists(PluginDirectory))
                {
                    Logger.Log("No plugins directory.", LoggedImportance.Warning);

                    return;
                }

                dllFileNames = Directory.GetFiles(PluginDirectory, "*.dll");

                ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
                foreach (string dllFile in dllFileNames)
                {
                    Assembly assembly = Assembly.LoadFrom(dllFile);
                    assemblies.Add(assembly);
                }

                Type pluginType = typeof(T);
                Dictionary<Assembly, ICollection<Type>> pluginsPerAssembly = new Dictionary<Assembly, ICollection<Type>>();
                ICollection<Type> pluginTypes = new List<Type>();
                foreach (Assembly assembly in assemblies)
                {
                    if (assembly != null)
                    {
                        Logger.Log($"Loading plugin { assembly.FullName } ...");

                        Type[] types = assembly.GetTypes();
                        foreach (Type type in types)
                        {
                            if (type.IsInterface || type.IsAbstract)
                            {
                                continue;
                            }
                            else
                            {
                                if (pluginType.IsAssignableFrom(type))
                                {
                                    if (!pluginsPerAssembly.ContainsKey(assembly))
                                        pluginsPerAssembly.Add(assembly, new List<Type>());

                                    pluginsPerAssembly[assembly].Add(type);
                                    pluginTypes.Add(type);
                                }
                            }
                        }
                    }
                }

                Logger.Log("Loaded plugins:");

                LoadedPlugins = new List<T>(pluginTypes.Count);
                foreach (Assembly a in pluginsPerAssembly.Keys)
                {
                    ICollection<Type> types = pluginsPerAssembly[a];

                    foreach(Type type in types)
                    {
                        T plugin = (T)Activator.CreateInstance(type);

                        if (PluginPrepare != null)
                            PluginPrepare.Invoke(plugin);

                        Logger.Log($"\t- { a.GetName().Name }->{ plugin.PluginName?.Trim() }({ plugin.PluginVersion?.Trim() })");

                        if (string.IsNullOrEmpty(plugin.PluginName) || string.IsNullOrEmpty(plugin.PluginVersion))
                        {
                            Logger.Log($"Invalid name or version given for plugin '{ plugin.GetType().Name }'. Unloading.", LoggedImportance.Warning);
                        }
                        else
                        {
                            LoadedPlugins.Add(plugin);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e, "An exception was thrown during plugin loading.");
            }
        }

        public bool SetFor<U>(string pluginName, string key, U value)
        {
            T p = LoadedPlugins.FirstOrDefault((e) => e.PluginName == pluginName);

            if (p is IConfiguredPlugin)
            {
                ((IConfiguredPlugin)p).Set<U>(key, value);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SetFor<U>(IPlugin plugin, string key, U value)
        {
            T p = LoadedPlugins.FirstOrDefault((e) => e.GetType() == plugin.GetType());

            if (p is IConfiguredPlugin)
            {
                ((IConfiguredPlugin)p).Set<U>(key, value);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reload()
        {
            DisablePlugins();
            Load();
            EnablePlugins();
        }

        public void EnablePlugins()
        {
            foreach (T v in LoadedPlugins)
                EnablePlugin(v);
        }

        public void EnablePlugin(T plugin)
        {
            try
            {
                if (!plugin.Enabled)
                {
                    plugin.Enabled = true;
                    plugin.OnEnable();
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e, "Could not enable plugin " + plugin?.PluginName);
            }
        }

        public void DisablePlugins()
        {
            foreach(T v in LoadedPlugins)
                DisablePlugin(v, false);
        }

        public void DisablePlugin(T plugin, bool remove = true)
        {
            try
            {
                if (plugin.Enabled)
                {
                    plugin.Enabled = false;
                    plugin.OnDisable();
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e, "Could not disable plugin " + plugin?.PluginName);
            }

            if (LoadedPlugins.Contains(plugin) && remove)
                LoadedPlugins.Remove(plugin);
        }
    }
}
