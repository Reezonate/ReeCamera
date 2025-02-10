using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ReeCamera {
    public readonly struct ScenePresetV1 : IScenePreset {
        #region Impl

        public const int FormatVersion = 1;
        public IReadOnlyList<SceneLayoutConfig> LayoutConfigs { get; }

        public ScenePresetV1(IReadOnlyList<SceneLayoutConfig> layoutConfigs) {
            LayoutConfigs = layoutConfigs;
        }

        #endregion

        #region Serialize

        public JObject Serialize() {
            return new JObject {
                { "FormatVersion", FormatVersion },
                { "ModVersion", Plugin.ModVersion },
                { "Layouts", JArray.FromObject(LayoutConfigs, ConfigUtils.DefaultSerializer) }
            };
        }

        #endregion

        #region Deserialize

        public static bool TryDeserialize(JObject input, out IScenePreset preset, out string failReason) {
            if (!(input.GetValue("FormatVersion") is JValue versionToken)) {
                failReason = "Corrupted config format";
                preset = default;
                return false;
            }

            failReason = null;
            preset = new ScenePresetV1(
                input.DeserializeOrDefault<IReadOnlyList<SceneLayoutConfig>>("Layouts", Array.Empty<SceneLayoutConfig>)
            );
            return true;
        }

        #endregion
    }
}