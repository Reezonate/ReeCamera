using Newtonsoft.Json;
using UnityEngine;

namespace ReeCamera {
    public struct CompositionSettings {
        public Rect ScreenRect;
        public string MaskTextureId;
        public bool Transparent;
        public Color BackgroundColor;
        public int BackgroundBlurLevel;
        public float BackgroundBlurScale;

        [JsonConstructor]
        public CompositionSettings(int _) {
            ScreenRect = new Rect(0f, 0f, 1f, 1f);
            MaskTextureId = string.Empty;
            Transparent = true;
            BackgroundColor = Color.white;
            BackgroundBlurLevel = 0;
            BackgroundBlurScale = 1.0f;
        }

        public static CompositionSettings Default => new CompositionSettings(0);
    }
}