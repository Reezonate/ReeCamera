using UnityEngine;

namespace ReeCamera.Spout {
    [AddComponentMenu("Spout/TextureSpoutReceiver")]
    public class TextureSpoutReceiver : AbstractSpoutReceiver {
        public RenderTexture targetTexture;

        protected override void Update() {
            base.Update();

            // Texture format conversion with the blit shader
            if (!SharedTextureInitialized) return;

            InitializeBlitMaterial();
            Graphics.Blit(SharedTexture, targetTexture, BlitMaterial, 1);
        }
    }
}