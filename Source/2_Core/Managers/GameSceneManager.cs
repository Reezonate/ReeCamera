using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace ReeCamera {
    public class GameSceneManager : MonoBehaviour {
        [Inject, UsedImplicitly]
        private PlayerTransforms _playerTransforms;

        [Inject, UsedImplicitly]
        private BeatmapObjectManager _beatmapObjectManager;

        private void Awake() {
            PluginState.SceneTypeOV.SetValue(SceneType.Gameplay, this);

            switch (PluginState.LaunchTypeOV.Value) {
                case LaunchType.VR: {
                    var config = MainPluginConfig.Instance.GameplayConfigVR;
                    ReeSceneController.Instantiate(transform, config);
                    break;
                }
                case LaunchType.FPFC:
                default: {
                    var config = MainPluginConfig.Instance.GameplayConfigFPFC;
                    FramerateManager.Instantiate(gameObject, config.FramerateSettingsOV);
                    ReeSceneController.Instantiate(transform, config);
                    break;
                }
            }
        }

        private void Start() {
            _beatmapObjectManager.noteWasCutEvent += HandleNoteWasCut;
        }

        private void OnDestroy() {
            PluginState.SceneTypeOV.SetValue(SceneType.MainMenu, this);

            _beatmapObjectManager.noteWasCutEvent -= HandleNoteWasCut;
        }

        private static void HandleNoteWasCut(NoteController noteController, in NoteCutInfo noteCutInfo) {
            PluginState.NotifyNoteWasCut();
        }

        private void LateUpdate() {
            var fpvPose = ReeTransform.FromTransform(_playerTransforms._headTransform);
            PluginState.FirstPersonPoseOV.SetValue(fpvPose, this);
        }
    }
}