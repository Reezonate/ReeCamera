using UnityEngine;

namespace ReeCamera.Spout {
    [AddComponentMenu("Spout/RendererSpoutReceiver")]
    public class RendererSpoutReceiver : AbstractSpoutReceiver {
        public Renderer targetRenderer;
        public string targetMaterialProperty;

        private MaterialPropertyBlock _propertyBlock;

        protected override void Update() {
            base.Update();

            // Texture format conversion with the blit shader
            if (SharedTextureInitialized) {
                InitializeBlitMaterial();
                InitializeReceivedTexture();
                Graphics.Blit(SharedTexture, ReceivedTexture, BlitMaterial, 1);
            }

            // Renderer override
            if (!ReceivedTextureInitialized) return;

            if (_propertyBlock == null) _propertyBlock = new MaterialPropertyBlock();
            targetRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetTexture(targetMaterialProperty, ReceivedTexture);
            targetRenderer.SetPropertyBlock(_propertyBlock);
        }
    }
}