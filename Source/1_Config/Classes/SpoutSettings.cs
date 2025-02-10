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
}