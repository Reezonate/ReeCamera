using UnityEngine;
using Input = UnityEngine.Input;

namespace ReeCamera {
    public class PluginStateManager : MonoBehaviour {
        #region Init / Dispose

        private MainSettingsModelSO _mainSettingsModel;

        private void Start() {
            var screenCanvas = gameObject.AddComponent<Canvas>();
            screenCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            PluginState.ScreenCanvasOV.SetValue(screenCanvas, this);

            _mainSettingsModel = PluginState.MainSettingsModelOV.Value;
            _mainSettingsModel.windowResolution.didChangeEvent += OnResolutionChanged;
            OnResolutionChanged();
            PluginState.SceneTypeOV.AddStateListener(OnSceneTypeChanged, this);
        }

        private void OnDestroy() {
            _mainSettingsModel.windowResolution.didChangeEvent -= OnResolutionChanged;
            PluginState.SceneTypeOV.RemoveStateListener(OnSceneTypeChanged);
        }

        #endregion

        #region Hotkeys

        private bool _refreshPressed;

        private void Update() {
            var pressed = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                          && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                          && Input.GetKeyDown(KeyCode.F1);

            if (_refreshPressed != pressed) {
                _refreshPressed = pressed;

                if (pressed) {
                    MainPluginConfig.MassReload();
                    Plugin.Notice("Plugin settings have been reloaded.");
                }
            }
        }

        #endregion

        #region Events

        private void OnResolutionChanged() {
            var value = _mainSettingsModel.windowResolution.value;

            var resolution = Screen.currentResolution;
            resolution.width = value.x;
            resolution.height = value.y;

            PluginState.ScreenResolution.SetValue(resolution, this);
        }

        private void OnSceneTypeChanged(SceneType value, ObservableValueState state) {
            PluginState.ScreenCanvasOV.Value.enabled = value != SceneType.BeatmapEditor;
        }

        #endregion
    }
}