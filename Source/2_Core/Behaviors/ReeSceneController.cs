using System.Collections.Generic;
using UnityEngine;

namespace ReeCamera {
    public class ReeSceneController : MonoBehaviour {
        #region Instantiate

        public static ReeSceneController Instantiate(Transform parent, AbstractSceneConfig config) {
            var go = new GameObject("ReeSceneController");
            go.transform.SetParent(parent, false);

            var component = go.AddComponent<ReeSceneController>();
            component.Construct(config);
            return component;
        }

        #endregion

        #region Construct / Init / Dispose

        public AbstractSceneConfig Config { get; private set; }

        private void Construct(AbstractSceneConfig config) {
            Config = config;
        }

        private void Start() {
            Config.SecondaryConfigs.AddStateListener(OnSecondaryCamsChanged);
            PluginState.CameraPrefabOV.AddStateListener(OnCameraPrefabChanged, this);
            MainPluginConfig.Instance.Output.AntiAliasingOV.AddStateListener(OnAntiAliasingChanged, this);
        }

        private void OnDestroy() {
            Config.SecondaryConfigs.RemoveStateListener(OnSecondaryCamsChanged);
            PluginState.CameraPrefabOV.RemoveStateListener(OnCameraPrefabChanged);
            MainPluginConfig.Instance.Output.AntiAliasingOV.RemoveStateListener(OnAntiAliasingChanged);

            UnsubscribeSecondary();
        }

        private void Update() {
            UpdateMainCameraIfDirty();
            UpdateSecondaryCamerasIfDirty();
            UpdateCompositionIfDirty();
        }

        #endregion

        #region Events

        private int _antiAliasing = 1;

        private void OnAntiAliasingChanged(int value, ObservableValueState state) {
            _antiAliasing = value;
            MarkCompositionDirty();
        }

        private void OnCompositionCameraChanged(CompositionSettings compositionSettings, ObservableValueState state) {
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
            _composition.AntiAliasing = _antiAliasing;
            _composition.enabled = _secondaryCameras.Count > 0;
            foreach (var secondaryCamera in _secondaryCameras) {
                var settings = secondaryCamera.Config.CompositionSettingsOV.Value;
                _composition.SetupCamera(secondaryCamera.Camera, settings);
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
                transform, Config.MainCameraConfig,
                PluginState.CameraPrefabOV.Value
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

            foreach (var config in Config.SecondaryConfigs.Items) {
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
                secondaryCamera.Config.CompositionSettingsOV.AddStateListener(OnCompositionCameraChanged, this);
            }

            _subscribedSecondary = true;
        }

        private void UnsubscribeSecondary() {
            if (!_subscribedSecondary) return;

            foreach (var secondaryCamera in _secondaryCameras) {
                secondaryCamera.Config.CompositionSettingsOV.RemoveStateListener(OnCompositionCameraChanged);
            }

            _subscribedSecondary = false;
        }

        #endregion
    }
}