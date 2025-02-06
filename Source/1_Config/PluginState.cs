using System;
using UnityEngine;

namespace ReeCamera {
    public static class PluginState {
        public static readonly IObservableValue<LaunchType> LaunchTypeOV = new ObservableValue<LaunchType>(LaunchType.FPFC);
        public static readonly IObservableValue<SceneType> SceneTypeOV = new ObservableValue<SceneType>(SceneType.MainMenu);
        public static readonly IObservableValue<GameObject> CameraPrefabOV = new ObservableValue<GameObject>();
        public static readonly IObservableValue<ReeTransform> FirstPersonPoseOV = new ObservableValue<ReeTransform>();
        public static readonly IObservableValue<Resolution> ScreenResolution = new ObservableValue<Resolution>();
        public static readonly IObservableValue<RenderTexture> OutputTexture = new ObservableValue<RenderTexture>();

        public static event Action NoteWasCutEvent;

        public static void NotifyNoteWasCut() {
            NoteWasCutEvent?.Invoke();
        }
    }
}