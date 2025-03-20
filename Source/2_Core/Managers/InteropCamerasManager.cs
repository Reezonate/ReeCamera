using System.Linq;
using CameraUtils.Core;
using UnityEngine;

namespace ReeCamera {
    [RequireComponent(typeof(Camera)), DisallowMultipleComponent]
    public class InteropCameraDisabler : MonoBehaviour {
        private Camera _camera;

        private void Awake() {
            _camera = GetComponent<Camera>();
        }

        private void OnEnable() {
            _camera.enabled = false;
        }

        private void OnPreRender() {
            _camera.enabled = false;
        }
    }

    public class InteropCamerasManager : MonoBehaviour, ICameraEffect {
        #region Init / Dispose

        private void Start() {
            CamerasManager.RegisterCameraEffect(this);
        }

        private void OnDestroy() {
            CamerasManager.UnRegisterCameraEffect(this);
        }

        #endregion

        #region IcameraEffect Impl

        private static readonly string[] KnownCameras = {
            "MenuMainCamera",
            "MainCamera",
            "SmoothCamera",
            "RecorderCamera", //ScoreSaber replay
            "ReplayerViewCamera" //BeatLeader replay
        };

        public bool IsSuitableForCamera(RegisteredCamera registeredCamera) {
            if (registeredCamera.CameraFlags.HasFlag(CameraFlags.Mirror) || registeredCamera.Camera.stereoEnabled) return false;
            var go = registeredCamera.Camera.gameObject;
            if (go.GetComponent<MainCameraController>() != null || go.GetComponent<SecondaryCameraController>() != null) return false;
            return KnownCameras.Any(knownName => go.name.Contains(knownName));
        }

        public void HandleAddedToCamera(RegisteredCamera registeredCamera) {
            registeredCamera.Camera.gameObject.AddComponent<InteropCameraDisabler>();
        }

        public void HandleRemovedFromCamera(RegisteredCamera registeredCamera) {
            var disabler = registeredCamera.Camera.gameObject.GetComponent<InteropCameraDisabler>();
            if (disabler == null) return;
            Destroy(disabler);
        }

        #endregion
    }
}