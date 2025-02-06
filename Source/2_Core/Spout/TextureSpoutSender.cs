using UnityEngine;

namespace ReeCamera.Spout {
    [AddComponentMenu("Spout/TextureSpoutSender")]
    public class TextureSpoutSender : AbstractSpoutSender {
        public RenderTexture sourceTexture;

        protected override void Update() {
            base.Update();
            SendTextureMode(sourceTexture);
        }
    }
}