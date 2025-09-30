using System;
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
            switch (PluginState.LaunchTypeOV.Value) {
                case LaunchType.VR: {
                    var config = MainPluginConfig.Instance.GameplayConfigVR;
                    ReeSceneController.Instantiate(_playerTransforms._originTransform, config);
                    break;
                }
                case LaunchType.FPFC:
                default: {
                    var config = MainPluginConfig.Instance.GameplayConfigFPFC;
                    FramerateManager.Instantiate(gameObject, config.FramerateSettingsOV);
                    ReeSceneController.Instantiate(_playerTransforms._originTransform, config);
                    break;
                }
            }
        }

        private void OnEnable() {
            PluginState.SceneTypeOV.SetValue(SceneType.Gameplay, this);
        }

        private void Start() {
            _beatmapObjectManager.noteWasCutEvent += HandleNoteWasCut;
        }

        private void OnDestroy() {
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