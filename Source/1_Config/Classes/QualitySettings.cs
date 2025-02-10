using Newtonsoft.Json;

namespace ReeCamera {
    public struct QualitySettings {
        public int AntiAliasing;
        public float RenderScale;

        [JsonConstructor]
        public QualitySettings(int _) {
            AntiAliasing = 1;
            RenderScale = 1;
        }

        public static QualitySettings Default => new QualitySettings(0);
    }
}