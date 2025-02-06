using Newtonsoft.Json;

namespace ReeCamera {
    public struct SpoutSettings {
        public bool Enabled;
        public int Width;
        public int Height;
        public string ChannelName;

        [JsonConstructor]
        public SpoutSettings(int _) {
            Enabled = false;
            Width = 1920;
            Height = 1080;
            ChannelName = "ReeCamera";
        }

        public static SpoutSettings Default = new SpoutSettings(0);
    }

    public class OutputConfig {
        #region Values

        [JsonProperty("AntiAliasing"), JsonConverter(typeof(ObservableValue<int>.Converter))]
        public ObservableValue<int> AntiAliasingOV = new ObservableValue<int>(1);

        [JsonProperty("SpoutSettings"), JsonConverter(typeof(ObservableValue<SpoutSettings>.Converter))]
        public ObservableValue<SpoutSettings> SpoutSettingsOV = new ObservableValue<SpoutSettings>(SpoutSettings.Default);

        #endregion
    }
}