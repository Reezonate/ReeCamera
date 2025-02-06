using UnityEngine;

namespace ReeCamera {
    public class MainCameraController : AbstractCameraController<MainCameraConfig> {
        #region Instantiate

        public static MainCameraController Instantiate(Transform parent, MainCameraConfig config, GameObject cameraPrefab) {
            var go = Instantiate(cameraPrefab, parent, false);
            var camera = go.GetComponent<Camera>();
            go.SetActive(true);

            var component = go.AddComponent<MainCameraController>();
            component.Construct(config, camera);
            return component;
        }

        #endregion

        #region Init / Dispose

        protected override void Start() {
            base.Start();
            PluginState.OutputTexture.AddStateListener(OnOutputTextureChanged, this);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            PluginState.OutputTexture.RemoveStateListener(OnOutputTextureChanged);
        }

        #endregion

        #region Events

        private void OnOutputTextureChanged(RenderTexture value, ObservableValueState state) {
            Camera.targetTexture = value;
        }

        #endregion
    }
}