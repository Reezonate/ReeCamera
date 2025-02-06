using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace ReeCamera {
    public class MenuSceneManager : MonoBehaviour {
        [Inject, UsedImplicitly]
        private MainCamera _mainCamera;

        private void Awake() {
            switch (PluginState.LaunchTypeOV.Value) {
                case LaunchType.VR: {
                    var config = MainPluginConfig.Instance.MainMenuConfigVR;
                    ReeSceneController.Instantiate(transform, config);
                    break;
                }
                case LaunchType.FPFC:
                default: {
                    var config = MainPluginConfig.Instance.MainMenuConfigFPFC;
                    FramerateManager.Instantiate(gameObject, config.FramerateSettingsOV);
                    ReeSceneController.Instantiate(transform, config);
                    break;
                }
            }
        }

        private void LateUpdate() {
            var fpvPose = ReeTransform.FromTransform(_mainCamera.transform);
            PluginState.FirstPersonPoseOV.SetValue(fpvPose, this);
        }
    }
}