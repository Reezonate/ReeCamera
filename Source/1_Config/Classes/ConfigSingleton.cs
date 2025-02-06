using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ReeCamera {
    public abstract class ConfigSingleton<T> where T : ConfigSingleton<T>, new() {
        #region Load / Save

        public static T Instance { get; private set; }

        public static void Load(string filename) {
            try {
                var jObject = JObject.Parse(File.ReadAllText(filename));
                Instance = jObject.ToObject<T>();
            } catch (Exception ex) {
                Instance = new T();
                Plugin.Error($"{typeof(T).Name} load failed! {ex}");
            }
        }

        public static void Save(string filename) {
            try {
                ConfigUtils.EnsureDirectoryExists(filename);
                var jObject = ConfigUtils.SerializeObject(Instance);
                File.WriteAllText(filename, jObject.ToString());
            } catch (Exception ex) {
                Plugin.Error($"{typeof(T).Name} save failed! {ex.Message}");
                throw;
            }
        }

        #endregion
    }
}