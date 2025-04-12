using Newtonsoft.Json;

namespace ReeCamera {
    public struct FramerateSettings {
        public bool VSync;
        public int TargetFramerate;

        [JsonConstructor]
        public FramerateSettings(int _) {
            VSync = true;
            TargetFramerate = 0;
        }

        public static FramerateSettings Default = new FramerateSettings(0);
        
        public static FramerateSettings NoVSync60Fps = new FramerateSettings {
            VSync = false,
            TargetFramerate = 60
        };
    }
}