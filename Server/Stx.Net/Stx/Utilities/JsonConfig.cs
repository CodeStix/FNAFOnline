using Newtonsoft.Json;
using Stx.Logging;
using Stx.Utilities.ErrorHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Stx.Utilities
{
    public class JsonConfig<T> where T : new()
    {
        public string FileName { get; set; }
        public T Settings { get; set; }
        public bool IsFirstLoad { get; } = false;

        public delegate void FirstTimeDelegate();
        public event FirstTimeDelegate OnFirstTime;

        public delegate void BeforeSaveDelegate();
        public event BeforeSaveDelegate OnSave;

        public ILogger Logger { get; set; } = new VoidLogger();

        public JsonConfig(string fileName)
        {
            this.FileName = fileName;
            this.Settings = new T();

            string dir = new FileInfo(fileName).Directory.FullName;

            if (!Directory.Exists(fileName))
                Directory.CreateDirectory(dir);

            if (File.Exists(FileName))
            {
                Load();
            }
            else
            {
                IsFirstLoad = true;
                OnFirstTime?.Invoke();

                Save();
            }
        }

        ~JsonConfig()
        {
            Save();
        }

        public void Save()
        {
            OnSave?.Invoke();

            try
            {
                File.WriteAllText(FileName, JsonConvert.SerializeObject(Settings));
            }
            catch(Exception e)
            {
                Logger.LogException(e, "Could not save json config file");
            }
        }

        public void Load()
        {
            try
            {
                Settings = JsonConvert.DeserializeObject<T>(File.ReadAllText(FileName));
            }
            catch (Exception e)
            {
                Logger.LogException(e, "Could not load json config file");
            }
        }
    }
}
