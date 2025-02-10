using System.Collections;
using UnityEngine;

namespace ReeCamera {
    public class FramerateManager : MonoBehaviour {
        #region Instantiate

        public static FramerateManager Instantiate(GameObject gameObject, IObservableValue<FramerateSettings> framerateSettingsOV) {
            var component = gameObject.AddComponent<FramerateManager>();
            component.Construct(framerateSettingsOV);
            return component;
        }

        #endregion

        #region Start / Destroy

        public IObservableValue<FramerateSettings> FramerateSettingsOV { get; private set; }

        private void Construct(IObservableValue<FramerateSettings> framerateSettingsOV) {
            FramerateSettingsOV = framerateSettingsOV;
        }

        private void Start() {
            FramerateSettingsOV.AddStateListener(OnFramerateSettingsChanged, this);
        }

        private void OnDestroy() {
            FramerateSettingsOV.RemoveStateListener(OnFramerateSettingsChanged);
        }

        private void Update() {
            UpdateIfDirty();
        }

        #endregion

        #region Events

        private FramerateSettings _settings;

        private void OnFramerateSettingsChanged(FramerateSettings value, ObservableValueState state) {
            _settings = value;
            MarkDirty();
        }

        private void OnEnable() {
            StartCoroutine(SceneStartCoroutine());
        }

        private IEnumerator SceneStartCoroutine() {
            yield return new WaitForEndOfFrame();
            MarkDirty();

            for (var i = 0; i < 4; i++) {
                yield return new WaitForSeconds(1);
                MarkDirty();
            }
        }

        #endregion

        #region UpdateIfDirty

        private bool _isDirty;

        private void MarkDirty() {
            _isDirty = true;
        }

        private void UpdateIfDirty() {
            if (!_isDirty) return;
            _isDirty = false;

            var screenRefreshRate = Screen.currentResolution.refreshRate;

            if (_settings.VSync) {
                UnityEngine.QualitySettings.vSyncCount = 1;
                ReeSabersInterop.SetTargetFramerate(screenRefreshRate);
            } else {
                UnityEngine.QualitySettings.vSyncCount = 0;

                if (_settings.TargetFramerate > 0) {
                    Application.targetFrameRate = _settings.TargetFramerate;
                    ReeSabersInterop.SetTargetFramerate(_settings.TargetFramerate);
                } else {
                    Application.targetFrameRate = 0;
                    ReeSabersInterop.SetTargetFramerate(screenRefreshRate);
                }
            }
        }

        #endregion
    }
}