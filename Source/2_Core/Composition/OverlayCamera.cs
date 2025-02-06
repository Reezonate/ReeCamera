using UnityEngine;

namespace ReeCamera {
    public struct OverlayCamera {
        public Camera Camera;
        public Vector4 ScreenRect;
        public Texture2D MaskTexture;
        public bool IsTransparent;
        public Color BackgroundColor;
        public int BackgroundBlurLevel;
        public float BackgroundBlurScale;
    }
}