using System.Collections.Generic;
using UnityEngine;

namespace ReeCamera {
    public class ReeSceneController : MonoBehaviour {
        #region Instantiate

        public static ReeSceneController Instantiate(Transform parent, AbstractSceneConfig config) {
            var go = new GameObject("ReeSceneController");
            go.transform.SetParent(parent, false);

            var component = go.AddComponent<ReeSceneController>();
            component.Construct(config);
            return component;
        }

        #endregion

        #region Construct / Init / Dispose

        public AbstractSceneConfig Config { get; private set; }

        private void Construct(AbstractSceneConfig config) {
            Config = config;
        }

        private void Start() {
            Config.LayoutConfigs.AddStateListener(OnLayoutsChanged);
        }

        private void OnDestroy() {
            Config.LayoutConfigs.RemoveStateListener(OnLayoutsChanged);
            DisposeLayouts();
        }

        #endregion

        #region Events

        private readonly List<ReeLayoutController> _layoutControllers = new List<ReeLayoutController>();

        private void OnLayoutsChanged(IReadOnlyList<SceneLayoutConfig> configs) {
            DisposeLayouts();

            foreach (var config in configs) {
                var controller = ReeLayoutController.Instantiate(transform, config);
                _layoutControllers.Add(controller);
            }
        }

        private void DisposeLayouts() {
            foreach (var layoutController in _layoutControllers) {
                DestroyImmediate(layoutController.gameObject);
            }

            _layoutControllers.Clear();
        }

        #endregion
    }
}