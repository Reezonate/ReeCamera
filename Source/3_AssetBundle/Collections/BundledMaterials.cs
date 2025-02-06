using UnityEngine;

namespace ReeCamera {
    [CreateAssetMenu(fileName = "BundledMaterials", menuName = "Ree/BundledMaterials")]
    public class BundledMaterials : ScriptableObject {
        public Material spoutBlitMaterial;
        public Material screenImageMaterial;
    }
}