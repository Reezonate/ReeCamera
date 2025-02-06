using UnityEngine;
using UnityEngine.Rendering;

namespace ReeCamera {
    public struct CompositionCamera {
        public Camera Camera;
        public Vector4 ScreenRect;
        public Texture2D MaskTexture;
        public RenderTexture TargetTexture;
        public Material ClearMaterial;
        public CommandBuffer ClearCommandBuffer;

        public void Dispose() {
            TargetTexture.Release();
            ClearCommandBuffer?.Release();
            Object.Destroy(ClearMaterial);
        }
    }
}