using System.Collections.Generic;
using UnityEngine;

namespace ReeCamera {
    public class CameraPrefabManager : MonoBehaviour {
        private GameObject _cameraPrefabGameObject;

        private readonly HashSet<string> _allowedComponents = [
            "UnityEngine.Transform",
            "UnityEngine.Camera",
            "MainEffectController",
            "ImageEffectController",
            "BloomPrePass",
            "CameraUtils.Behaviours.AutoCameraRegistrator"
        ];

        private void Start() {
            var tmp = GameObject.Find("SmoothCamera");

            _cameraPrefabGameObject = Instantiate(tmp, null, false);
            _cameraPrefabGameObject.name = "ReeCameraPrefab";
            _cameraPrefabGameObject.SetActive(false);
            DontDestroyOnLoad(_cameraPrefabGameObject);

            foreach (var component in _cameraPrefabGameObject.GetComponents<Behaviour>()) {
                if (_allowedComponents.Contains(component.GetType().FullName)) {
                    component.enabled = true;
                    continue;
                }

                DestroyImmediate(component);
            }

            PluginState.CameraPrefabOV.SetValue(_cameraPrefabGameObject, this);
        }

        private void OnDestroy() {
            Destroy(_cameraPrefabGameObject);
        }
    }
}