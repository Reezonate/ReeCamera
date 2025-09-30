using BeatSaber.Settings;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using Input = UnityEngine.Input;

namespace ReeCamera {
    public class PluginStateManager : MonoBehaviour {
        #region Static

        internal static readonly ObservableValue<Settings> BaseGameSettingOV = new ObservableValue<Settings>();

        #endregion

        #region Init / Dispose

        [Inject, UsedImplicitly]
        private SettingsApplicatorSO _settingsApplicator;

        private void Start() {
            var screenCanvas = gameObject.AddComponent<Canvas>();
            screenCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            PluginState.ScreenCanvasOV.SetValue(screenCanvas, this);

            BaseGameSettingOV.AddStateListener(OnBaseGameSettingsDidChange, this);
            PluginState.SceneTypeOV.AddStateListener(OnSceneTypeChanged, this);
        }

        private void OnDestroy() {
            BaseGameSettingOV.RemoveStateListener(OnBaseGameSettingsDidChange);
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

        private void OnBaseGameSettingsDidChange(Settings value, ObservableValueState state) {
            var resolution = Screen.currentResolution;
            resolution.width = value.window.resolution.x;
            resolution.height = value.window.resolution.y;

            PluginState.ScreenResolution.SetValue(resolution, this);
        }

        private void OnSceneTypeChanged(SceneType value, ObservableValueState state) {
            PluginState.ScreenCanvasOV.Value.enabled = value != SceneType.BeatmapEditor;
        }

        #endregion
    }
}