using Newtonsoft.Json;

namespace ReeCamera {
    public class VRSceneConfig : AbstractSceneConfig {
        #region Default

        public static VRSceneConfig Default => new VRSceneConfig();

        #endregion
    }

    public class FPFCSceneConfig : AbstractSceneConfig {
        #region Values

        [JsonProperty("FramerateSettings"), JsonConverter(typeof(ObservableValue<FramerateSettings>.Converter))]
        public ObservableValue<FramerateSettings> FramerateSettingsOV = new ObservableValue<FramerateSettings>(FramerateSettings.NoVSync60Fps);

        #endregion

        #region Default

        public static FPFCSceneConfig Default => new FPFCSceneConfig();

        #endregion
    }

    public abstract class AbstractSceneConfig {
        #region Values

        [JsonProperty("PresetId"), JsonConverter(typeof(ObservableValue<string>.Converter))]
        public ObservableValue<string> PresetIdOV = new ObservableValue<string>(string.Empty);

        #endregion

        #region Non-Serializable Properties

        [JsonIgnore]
        public readonly ReeObservableList<SceneLayoutConfig> LayoutConfigs = new ReeObservableList<SceneLayoutConfig>();

        #endregion

        #region Events

        protected AbstractSceneConfig() {
            PresetIdOV.AddStateListener(OnPresetIdChanged, this);
        }

        private void OnPresetIdChanged(string value, ObservableValueState state) {
            var option = PresetsStorage.Instance.GetOptionOrDefault(value);
            if (option == null) return;

            if (!option.TryLoad(out var preset, out var failReason)) {
                Plugin.Error($"Unable to load preset {value}! Reason: {failReason}");
                return;
            }

            ApplyPreset(preset);
        }

        #endregion

        #region Preset

        public void ReloadPreset() {
            OnPresetIdChanged(PresetIdOV.Value, ObservableValueState.Final);
        }

        public ScenePresetV1 CreatePreset() {
            return new ScenePresetV1(LayoutConfigs.Items);
        }

        public void ApplyPreset(IScenePreset preset) {
            LayoutConfigs.CopyFrom(preset.LayoutConfigs);
        }

        #endregion
    }
}