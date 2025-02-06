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
            BaseGameSettingOV.AddStateListener(OnBaseGameSettingsDidChange, this);
        }

        private void OnDestroy() {
            BaseGameSettingOV.RemoveStateListener(OnBaseGameSettingsDidChange);
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

        #endregion
    }
}