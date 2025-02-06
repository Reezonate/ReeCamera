using System.Collections.Generic;
using Newtonsoft.Json;
using ReeSabers.Storages;

namespace ReeCamera {
    public class MainPluginConfig : ConfigSingleton<MainPluginConfig> {
        #region Values

        public readonly OutputConfig Output = new OutputConfig();

        public readonly VRSceneConfig MainMenuConfigVR = VRSceneConfig.Default;
        public readonly VRSceneConfig GameplayConfigVR = VRSceneConfig.Default;

        public readonly FPFCSceneConfig MainMenuConfigFPFC = FPFCSceneConfig.Default;
        public readonly FPFCSceneConfig GameplayConfigFPFC = FPFCSceneConfig.Default;

        [JsonIgnore]
        public IEnumerable<AbstractSceneConfig> AllConfigs => new List<AbstractSceneConfig> {
            MainMenuConfigVR,
            GameplayConfigVR,
            MainMenuConfigFPFC,
            GameplayConfigFPFC
        };

        #endregion

        #region Mass Reload / Save

        public static void MassReload() {
            PresetsStorage.Instance.Refresh();
            CustomTexturesStorage.Instance.Refresh();

            foreach (var config in Instance.AllConfigs) {
                config.ReloadPreset();
            }
        }

        public static void SaveAll() {
            foreach (var option in PresetsStorage.Instance.GetAllOptions()) {
                if (!option.TryLoad(out var result, out _)) continue;
                if (result is not ScenePresetV1 v1) continue;

                switch (option.Id) {
                    case "":
                    case null:
                        continue;
                    default:
                        PresetsStorage.Instance.TryWritePreset(v1, option.Id.Replace(".json", ""));
                        break;
                }
            }

            Save(Plugin.MainConfigPath);
        }

        #endregion
    }
}