using System.Collections.Generic;
using ReeSabers.Storages;
using UnityEngine;

namespace ReeCamera {
    [DisallowMultipleComponent]
    public class CompositionImageEffect : MonoBehaviour {
        #region Cameras

        private readonly Dictionary<Camera, OverlayCamera> _cameras = new Dictionary<Camera, OverlayCamera>();

        public void SetupCamera(Camera cam, in CompositionSettings settings) {
            var maskTexture = CustomTexturesStorage.Instance.LoadOrError(settings.MaskTextureId);
            var screenRectV4 = new Vector4(settings.ScreenRect.x, settings.ScreenRect.y, settings.ScreenRect.width, settings.ScreenRect.height);
            _cameras[cam] = new OverlayCamera {
                Camera = cam,
                ScreenRect = screenRectV4,
                MaskTexture = maskTexture,
                IsTransparent = settings.Transparent,
                BackgroundColor = settings.BackgroundColor,
                BackgroundBlurLevel = settings.BackgroundBlurLevel,
                BackgroundBlurScale = settings.BackgroundBlurScale
            };
            MarkIsDirty();
        }

        public void Clear() {
            _cameras.Clear();
            MarkIsDirty();
        }

        public void RemoveCamera(Camera cam) {
            if (!_cameras.ContainsKey(cam)) return;
            _cameras.Remove(cam);
            MarkIsDirty();
        }

        #endregion

        #region Settings

        private int _antiAliasing;

        public int AntiAliasing {
            get => _antiAliasing;
            set {
                _antiAliasing = value;
                MarkIsDirty();
            }
        }

        #endregion

        #region Events

        private void OnRenderImage(RenderTexture src, RenderTexture dest) {
            UpdateComposition(src.width, src.height);
            _composition.Render(src, dest);
        }

        private void OnDestroy() {
            Dispose();
        }

        #endregion

        #region Composition

        private Composition _composition;
        private bool _isDirty = true;

        private void UpdateComposition(int screenWidth, int screenHeight) {
            if (!_isDirty && _composition != null && _composition.VerifyScreenSize(screenWidth, screenHeight)) return;
            Dispose();
            _composition = BundleLoader.CompositionManager.CreateComposition(_cameras.Values, screenWidth, screenHeight, AntiAliasing);
            _isDirty = false;
        }

        private void MarkIsDirty() {
            _isDirty = true;
        }

        private void Dispose() {
            _composition?.Dispose();
        }

        #endregion
    }
}