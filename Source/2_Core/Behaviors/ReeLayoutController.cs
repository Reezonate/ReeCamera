using System.Collections.Generic;
using UnityEngine;

namespace ReeCamera {
    public class ReeLayoutController : MonoBehaviour {
        #region Instantiate

        public static ReeLayoutController Instantiate(Transform parent, SceneLayoutConfig config) {
            var go = new GameObject("ReeLayoutController");
            go.transform.SetParent(parent, false);

            var component = go.AddComponent<ReeLayoutController>();
            component.Construct(config);
            return component;
        }

        #endregion

        #region Construct / Init / Dispose

        public SceneLayoutConfig Config { get; private set; }

        private void Construct(SceneLayoutConfig config) {
            Config = config;
        }

        private void Start() {
            Config.SecondaryCameras.AddStateListener(OnSecondaryCamsChanged);
            PluginState.CameraPrefabOV.AddStateListener(OnCameraPrefabChanged, this);
        }

        private void OnDestroy() {
            Config.SecondaryCameras.RemoveStateListener(OnSecondaryCamsChanged);
            PluginState.CameraPrefabOV.RemoveStateListener(OnCameraPrefabChanged);

            UnsubscribeSecondary();
        }

        private void Update() {
            UpdateMainCameraIfDirty();
            UpdateSecondaryCamerasIfDirty();
            UpdateCompositionIfDirty();
        }

        #endregion

        #region Events

        private void OnSecondaryCameraCompositionSettingsChanged(CompositionSettings compositionSettings, ObservableValueState state) {
            MarkCompositionDirty();
        }

        private void OnSecondaryCameraSettingsChanged(CameraSettings value, ObservableValueState state) {
            MarkCompositionDirty();
        }

        private void OnCameraPrefabChanged(GameObject o, ObservableValueState state) {
            MarkMainCameraDirty();
        }

        private void OnSecondaryCamsChanged(IReadOnlyList<SecondaryCameraConfig> list) {
            MarkSecondaryCamerasDirty();
            MarkCompositionDirty();
        }

        #endregion

        #region Composition

        private CompositionImageEffect _composition;
        private bool _compositionDirty;

        private void MarkCompositionDirty() {
            _compositionDirty = true;
        }

        private void UpdateCompositionIfDirty() {
            if (!_compositionDirty) return;
            _compositionDirty = false;

            var go = _mainCamera.Camera.gameObject;
            _composition = go.GetComponent<CompositionImageEffect>();
            if (_composition == null) {
                _composition = go.AddComponent<CompositionImageEffect>();
            }

            _composition.Clear();
            _composition.enabled = _secondaryCameras.Count > 0;
            foreach (var secondaryCamera in _secondaryCameras) {
                var compositionSettings = secondaryCamera.Config.CompositionSettingsOV.Value;
                var qualitySettings = secondaryCamera.Config.QualitySettingsOV.Value;
                _composition.SetupCamera(secondaryCamera.Camera, compositionSettings, qualitySettings);
            }
        }

        #endregion

        #region MainCamera

        private MainCameraController _mainCamera;
        private bool _mainCameraDirty;
        private bool _subscribedMain;

        private void MarkMainCameraDirty() {
            _mainCameraDirty = true;
        }

        private void UpdateMainCameraIfDirty() {
            if (!_mainCameraDirty) return;
            _mainCameraDirty = false;

            _mainCamera = MainCameraController.Instantiate(
                transform, Config.MainCamera,
                PluginState.CameraPrefabOV.Value,
                Config.ScreenRectOV,
                Config.IsVisibleOV
            );
        }

        #endregion

        #region SecondaryCameras

        private readonly List<SecondaryCameraController> _secondaryCameras = new List<SecondaryCameraController>();
        private bool _secondaryCamerasDirty;
        private bool _subscribedSecondary;

        private void MarkSecondaryCamerasDirty() {
            _secondaryCamerasDirty = true;
        }

        private void UpdateSecondaryCamerasIfDirty() {
            if (!_secondaryCamerasDirty) return;
            _secondaryCamerasDirty = false;

            UnsubscribeSecondary();

            foreach (var secondaryCamera in _secondaryCameras) {
                Destroy(secondaryCamera.gameObject);
            }

            _secondaryCameras.Clear();

            foreach (var config in Config.SecondaryCameras.Items) {
                var secondaryCamera = SecondaryCameraController.Instantiate(
                    transform, config,
                    PluginState.CameraPrefabOV.Value
                );

                _secondaryCameras.Add(secondaryCamera);
            }

            SubscribeSecondary();
        }

        private void SubscribeSecondary() {
            if (_subscribedSecondary) return;

            foreach (var secondaryCamera in _secondaryCameras) {
                secondaryCamera.Config.CompositionSettingsOV.AddStateListener(OnSecondaryCameraCompositionSettingsChanged, this);
                secondaryCamera.Config.CameraSettingsOV.AddStateListener(OnSecondaryCameraSettingsChanged, this);
            }

            _subscribedSecondary = true;
        }

        private void UnsubscribeSecondary() {
            if (!_subscribedSecondary) return;

            foreach (var secondaryCamera in _secondaryCameras) {
                secondaryCamera.Config.CompositionSettingsOV.RemoveStateListener(OnSecondaryCameraCompositionSettingsChanged);
                secondaryCamera.Config.CameraSettingsOV.RemoveStateListener(OnSecondaryCameraSettingsChanged);
            }

            _subscribedSecondary = false;
        }

        #endregion
    }
}