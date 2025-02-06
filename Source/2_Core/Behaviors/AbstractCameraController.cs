using CameraUtils.Behaviours;
using UnityEngine;

namespace ReeCamera {
    public abstract class AbstractCameraController<T> : MonoBehaviour where T : AbstractCameraConfig {
        #region Construct / Init / Dispose

        public T Config { get; private set; }
        public Camera Camera { get; private set; }
        public CameraMovementController CameraMovementController { get; private set; }
        public AutoCameraRegistrator AutoCameraRegistrator { get; private set; }

        protected void Construct(T config, Camera cam) {
            Config = config;
            Camera = cam;
            CameraMovementController = CameraMovementController.Instantiate(gameObject, Config);
        }

        private void Awake() {
            AutoCameraRegistrator = gameObject.GetComponent<AutoCameraRegistrator>();
        }

        protected virtual void Start() {
            Config.NameOV.AddStateListener(OnNameChanged, this);
            Config.CameraSettingsOV.AddStateListener(OnCameraSettingChanged, this);
            Config.LayerFilterOV.AddStateListener(OnLayerFilterChanged, this);
        }

        protected virtual void OnDestroy() {
            Config.NameOV.RemoveStateListener(OnNameChanged);
            Config.CameraSettingsOV.RemoveStateListener(OnCameraSettingChanged);
            Config.LayerFilterOV.RemoveStateListener(OnLayerFilterChanged);
        }

        #endregion

        #region Events

        private void OnNameChanged(string value, ObservableValueState state) {
            gameObject.name = value;
        }

        private void OnCameraSettingChanged(CameraSettings value, ObservableValueState state) {
            if (AutoCameraRegistrator != null) {
                AutoCameraRegistrator.enabled = !value.IgnoreCameraUtils;
            }
            
            Camera.fieldOfView = value.FieldOfView;
            Camera.nearClipPlane = value.NearClipPlane;
            Camera.farClipPlane = value.FarClipPlane;
            Camera.orthographic = value.Orthographic;
            Camera.orthographicSize = value.OrthographicSize;
        }

        private void OnLayerFilterChanged(LayerFilter value, ObservableValueState state) {
            Camera.cullingMask = value.CullingMask;
        }

        #endregion
    }
}