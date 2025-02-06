using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReeCamera {
    public class Composition : IDisposable {
        #region Constructor

        private readonly IReadOnlyList<CompositionCamera> _cameras;
        private readonly Material _compositionMaterial;
        private readonly int _screenWidth;
        private readonly int _screenHeight;

        public Composition(
            IReadOnlyList<CompositionCamera> cameras,
            Material compositionMaterial,
            int screenWidth,
            int screenHeight
        ) {
            _cameras = cameras;
            _compositionMaterial = compositionMaterial;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
        }

        #endregion

        #region Render

        public void Render(RenderTexture src, RenderTexture dest) {
            if (_cameras.Count == 0) {
                Graphics.Blit(src, dest);
                return;
            }

            foreach (var camera in _cameras) {
                camera.ClearMaterial.mainTexture = src;
                camera.Camera.Render();
            }

            Graphics.Blit(src, dest, _compositionMaterial);
        }

        #endregion

        #region VerifyScreenSize

        public bool VerifyScreenSize(int screenWidth, int screenHeight) {
            return screenWidth == _screenWidth && screenHeight == _screenHeight;
        }

        #endregion

        #region Dispose

        public void Dispose() {
            Object.Destroy(_compositionMaterial);
            foreach (var camera in _cameras) {
                camera.Dispose();
            }
        }

        #endregion
    }
}