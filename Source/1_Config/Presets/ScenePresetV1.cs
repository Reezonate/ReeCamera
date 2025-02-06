using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ReeCamera {
    public readonly struct ScenePresetV1 : IScenePreset {
        #region Impl

        public const int FormatVersion = 1;
        public MainCameraConfig MainCamera { get; }
        public IReadOnlyList<SecondaryCameraConfig> SecondaryCameras { get; }

        public ScenePresetV1(MainCameraConfig mainCamera, IReadOnlyList<SecondaryCameraConfig> secondaryCameras) {
            MainCamera = mainCamera;
            SecondaryCameras = secondaryCameras;
        }

        #endregion

        #region Serialize

        public JObject Serialize() {
            return new JObject {
                { "FormatVersion", FormatVersion },
                { "ModVersion", Plugin.ModVersion },
                { "MainCamera", JObject.FromObject(MainCamera, ConfigUtils.DefaultSerializer) },
                { "SecondaryCameras", JArray.FromObject(SecondaryCameras, ConfigUtils.DefaultSerializer) }
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
                input.DeserializeOrDefault("MainCamera", () => new MainCameraConfig()),
                input.DeserializeOrDefault<IReadOnlyList<SecondaryCameraConfig>>("SecondaryCameras", Array.Empty<SecondaryCameraConfig>)
            );
            return true;
        }

        #endregion
    }
}