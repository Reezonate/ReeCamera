using UnityEngine;

namespace ReeCamera {
    [CreateAssetMenu(fileName = "BundledTextures", menuName = "Ree/BundledTextures")]
    public class BundledTextures : ScriptableObject {
        public Texture2D missingTexture;
        public Texture2D whitePixel;
    }
}