using UnityEngine;

namespace ReeCamera {
    public class SecondaryCameraController : AbstractCameraController<SecondaryCameraConfig> {
        #region Instantiate

        public static SecondaryCameraController Instantiate(Transform parent, SecondaryCameraConfig config, GameObject cameraPrefab) {
            var go = Instantiate(cameraPrefab, parent, false);
            var camera = go.GetComponent<Camera>();
            go.SetActive(true);

            var component = go.AddComponent<SecondaryCameraController>();
            component.Construct(config, camera);
            return component;
        }

        #endregion
    }
}