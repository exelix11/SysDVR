using Newtonsoft.Json;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;

namespace SysDVRClientGUI.Logic
{
    internal class ConfigurationHandler<T> where T : class
    {
        public T Configuration { get; private set; }
        public string ConfigFilePath { get; init; }

        #region Constructor
        public ConfigurationHandler(string configFilePath)
        {
            this.ConfigFilePath = configFilePath;
        }
        #endregion

        public void Load()
        {
            if (!File.Exists(this.ConfigFilePath))
            {
                this.Configuration = Activator.CreateInstance<T>();
                this.Save();
            }

            string json = null;

            using (FileStream fs = File.Open(this.ConfigFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader r = new(fs))
                {
                    json = r.ReadToEnd();
                }
            }

            try
            {
                if (string.IsNullOrEmpty(json))
                {
                    throw new InvalidDataException("Empty file");
                }
                this.Configuration = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                Log.Debug($"Something went wrong: {ex.Message}");
                File.Delete(this.ConfigFilePath);
                this.Load();
            }
        }

        public void Save()
        {
            if (this.Configuration == default)
            {
                return;
            }

            TouchFile(this.ConfigFilePath);

            using (FileStream fs = File.Open(this.ConfigFilePath, FileMode.Truncate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (StreamWriter w = new(fs))
                {
                    w.Write(JsonConvert.SerializeObject(this.Configuration, Formatting.Indented));
                }
            }
        }

        private static void TouchFile(string filename)
        {
            if (!File.Exists(filename))
            {
                File.Create(filename).Dispose();
            }
        }
    }
}
