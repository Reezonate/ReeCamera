using UnityEngine;

namespace ReeCamera {
    public class BeatmapEditorSceneManager : MonoBehaviour {
        private void OnEnable() {
            PluginState.SceneTypeOV.SetValue(SceneType.BeatmapEditor, this);
        }
    }
}