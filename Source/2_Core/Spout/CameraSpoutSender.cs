using UnityEngine;

namespace ReeCamera.Spout {
    [AddComponentMenu("Spout/CameraSpoutSender")]
    public class CameraSpoutSender : AbstractSpoutSender {
        private void OnRenderImage(RenderTexture source, RenderTexture destination) {
            SendCameraMode(source, destination);
        }
    }
}