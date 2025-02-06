using Newtonsoft.Json;

namespace ReeCamera {
    public abstract class AbstractCameraConfig {
        #region Values

        [JsonProperty("Name"), JsonConverter(typeof(ObservableValue<string>.Converter))]
        public ObservableValue<string> NameOV = new ObservableValue<string>("Camera");

        [JsonProperty("CameraSettings"), JsonConverter(typeof(ObservableValue<CameraSettings>.Converter))]
        public ObservableValue<CameraSettings> CameraSettingsOV = new ObservableValue<CameraSettings>(CameraSettings.Default);

        [JsonProperty("MovementConfig"), JsonConverter(typeof(ObservableValue<MovementConfig>.Converter))]
        public ObservableValue<MovementConfig> MovementConfigOV = new ObservableValue<MovementConfig>(MovementConfig.Default);

        [JsonProperty("FollowTargetConfig"), JsonConverter(typeof(ObservableValue<FollowTargetConfig>.Converter))]
        public ObservableValue<FollowTargetConfig> FollowTargetConfigOV = new ObservableValue<FollowTargetConfig>(FollowTargetConfig.Default);

        // [JsonProperty("PhysicsLinkSettings"), JsonConverter(typeof(ObservableValue<PhysicsLinkSettings>.Converter))]
        // public ObservableValue<PhysicsLinkSettings> PhysicsLinkSettingsOV = new ObservableValue<PhysicsLinkSettings>(PhysicsLinkSettings.Default);

        [JsonProperty("LayerFilter"), JsonConverter(typeof(ObservableValue<LayerFilter>.Converter))]
        public ObservableValue<LayerFilter> LayerFilterOV = new ObservableValue<LayerFilter>(LayerFilter.Everything);

        #endregion

        #region CopyFrom

        public virtual void CopyFrom(AbstractCameraConfig other) {
            NameOV.SetValue(other.NameOV.Value, other.NameOV.LastChangeSource);
            CameraSettingsOV.SetValue(other.CameraSettingsOV.Value, other.CameraSettingsOV.LastChangeSource);
            MovementConfigOV.SetValue(other.MovementConfigOV.Value, other.MovementConfigOV.LastChangeSource);
            FollowTargetConfigOV.SetValue(other.FollowTargetConfigOV.Value, other.FollowTargetConfigOV.LastChangeSource);
            // PhysicsLinkSettingsOV.SetValue(other.PhysicsLinkSettingsOV.Value, other.PhysicsLinkSettingsOV.LastChangeSource);
            LayerFilterOV.SetValue(other.LayerFilterOV.Value, other.LayerFilterOV.LastChangeSource);
        }

        #endregion
    }

    public class MainCameraConfig : AbstractCameraConfig {
        #region Default

        public static MainCameraConfig Default => new MainCameraConfig();

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