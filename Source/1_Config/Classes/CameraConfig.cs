using Newtonsoft.Json;

namespace ReeCamera {
    public abstract class AbstractCameraConfig {
        #region Values

        [JsonProperty("Name"), JsonConverter(typeof(ObservableValue<string>.Converter))]
        public ObservableValue<string> NameOV = new ObservableValue<string>("Camera");

        [JsonProperty("QualitySettings"), JsonConverter(typeof(ObservableValue<QualitySettings>.Converter))]
        public ObservableValue<QualitySettings> QualitySettingsOV = new ObservableValue<QualitySettings>(QualitySettings.Default);

        [JsonProperty("CameraSettings"), JsonConverter(typeof(ObservableValue<CameraSettings>.Converter))]
        public ObservableValue<CameraSettings> CameraSettingsOV = new ObservableValue<CameraSettings>(CameraSettings.Default);

        [JsonProperty("MovementConfig"), JsonConverter(typeof(ObservableValue<MovementConfig>.Converter))]
        public ObservableValue<MovementConfig> MovementConfigOV = new ObservableValue<MovementConfig>(MovementConfig.Default);

        // [JsonProperty("PhysicsLinkSettings"), JsonConverter(typeof(ObservableValue<PhysicsLinkSettings>.Converter))]
        // public ObservableValue<PhysicsLinkSettings> PhysicsLinkSettingsOV = new ObservableValue<PhysicsLinkSettings>(PhysicsLinkSettings.Default);

        [JsonProperty("LayerFilter"), JsonConverter(typeof(ObservableValue<LayerFilter>.Converter))]
        public ObservableValue<LayerFilter> LayerFilterOV = new ObservableValue<LayerFilter>(LayerFilter.Default);

        #endregion

        #region CopyFrom

        public virtual void CopyFrom(AbstractCameraConfig other) {
            NameOV.SetValue(other.NameOV.Value, other.NameOV.LastChangeSource);
            CameraSettingsOV.SetValue(other.CameraSettingsOV.Value, other.CameraSettingsOV.LastChangeSource);
            QualitySettingsOV.SetValue(other.QualitySettingsOV.Value, other.QualitySettingsOV.LastChangeSource);
            MovementConfigOV.SetValue(other.MovementConfigOV.Value, other.MovementConfigOV.LastChangeSource);
            // PhysicsLinkSettingsOV.SetValue(other.PhysicsLinkSettingsOV.Value, other.PhysicsLinkSettingsOV.LastChangeSource);
            LayerFilterOV.SetValue(other.LayerFilterOV.Value, other.LayerFilterOV.LastChangeSource);
        }

        #endregion
    }

    public class MainCameraConfig : AbstractCameraConfig {
        #region Values

        [JsonProperty("SpoutSettings"), JsonConverter(typeof(ObservableValue<SpoutSettings>.Converter))]
        public ObservableValue<SpoutSettings> SpoutSettingsOV = new ObservableValue<SpoutSettings>(SpoutSettings.Default);

        #endregion

        #region Default

        public static MainCameraConfig Default => new MainCameraConfig();

        #endregion

        #region CopyFrom

        public override void CopyFrom(AbstractCameraConfig other) {
            base.CopyFrom(other);

            var tmp = (MainCameraConfig)other;
            SpoutSettingsOV.SetValue(tmp.SpoutSettingsOV.Value, tmp.SpoutSettingsOV.LastChangeSource);
        }

        #endregion
    }

    public class SecondaryCameraConfig : AbstractCameraConfig {
        #region Values

        [JsonProperty("CompositionSettings"), JsonConverter(typeof(ObservableValue<CompositionSettings>.Converter))]
        public ObservableValue<CompositionSettings> CompositionSettingsOV = new ObservableValue<CompositionSettings>(CompositionSettings.Default);

        #endregion

        #region CopyFrom

        public override void CopyFrom(AbstractCameraConfig other) {
            base.CopyFrom(other);

            var tmp = (SecondaryCameraConfig)other;
            CompositionSettingsOV.SetValue(tmp.CompositionSettingsOV.Value, tmp.CompositionSettingsOV.LastChangeSource);
        }

        #endregion
    }
}