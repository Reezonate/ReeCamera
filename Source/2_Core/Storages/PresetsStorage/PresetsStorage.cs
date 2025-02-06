using System;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace ReeCamera {
    [UsedImplicitly]
    public class PresetsStorage : AbstractFilesStorage<IScenePreset, PresetsStorage> {
        #region Built-In Options

        public static readonly BuiltInPresetStorageOption EmptyPresetOption = new BuiltInPresetStorageOption(
            "", "Default",
            new ScenePresetV1(
                MainCameraConfig.Default,
                Array.Empty<SecondaryCameraConfig>()
            )
        );

        #endregion

        #region TryLoadItem

        protected override string DirectoryPath { get; } = Path.Combine(Plugin.UserDataDirectory, "Presets");

        public bool TryWritePreset(ScenePresetV1 preset, string fileName) {
            try {
                var absolutePath = Path.Combine(DirectoryPath, $"{fileName}.json");
                File.WriteAllText(absolutePath, preset.Serialize().ToString());
            } catch (Exception e) {
                Plugin.Error($"Failed to write preset! {fileName} {e}");
                return false;
            }

            return true;
        }

        protected override bool TryLoadItem(string absolutePath, out IScenePreset result, out string failReason) {
            return TryDeserializePreset(absolutePath, out result, out failReason);
        }

        public static bool TryDeserializePreset(string absolutePath, out IScenePreset preset, out string failReason) {
            try {
                var jObject = JObject.Parse(File.ReadAllText(absolutePath));
                return ScenePresetV1.TryDeserialize(jObject, out preset, out failReason);
            } catch (Exception ex) {
                preset = default;
                failReason = ex.Message;
                return false;
            }
        }

        #endregion

        #region TryRemoveItem

        protected override bool TryRemoveItem(string absolutePath, out string failReason) {
            try {
                File.Delete(absolutePath);
                MarkOptionsDirty();
                failReason = "";
                return true;
            } catch (Exception ex) {
                failReason = ex.Message;
                return false;
            }
        }

        #endregion

        #region FillOptionsList

        protected override void FillOptionsList() {
            OptionsList.Add(EmptyPresetOption);
            base.FillOptionsList();
        }

        #endregion
    }
}