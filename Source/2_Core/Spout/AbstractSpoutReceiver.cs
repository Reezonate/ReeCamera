// KlakSpout - Spout video frame sharing plugin for Unity
// https://github.com/keijiro/KlakSpout

using System;
using UnityEngine;

namespace ReeCamera.Spout {
    public abstract class AbstractSpoutReceiver : MonoBehaviour {
        public Shader blitShader;
        public string sourceName;

        #region Private members

        private IntPtr _plugin;
        protected Texture2D SharedTexture;
        protected Material BlitMaterial;
        protected RenderTexture ReceivedTexture;
        protected bool ReceivedTextureInitialized;
        protected bool SharedTextureInitialized;
        protected bool BlitMaterialInitialized;
        protected bool PluginInitialized;

        #endregion

        #region MonoBehaviour implementation

        private void OnDisable() {
            if (PluginInitialized) {
                Util.IssuePluginEvent(PluginEntry.Event.Dispose, _plugin);
                _plugin = IntPtr.Zero;
                PluginInitialized = false;
            }

            Util.Destroy(SharedTexture);
            SharedTextureInitialized = false;
        }

        private void OnDestroy() {
            Util.Destroy(BlitMaterial);
            Util.Destroy(ReceivedTexture);
        }

        protected virtual void Update() {
            CheckPluginIsValid();
            InitializePlugin();

            Util.IssuePluginEvent(PluginEntry.Event.Update, _plugin);

            // Texture information retrieval
            var ptr = PluginEntry.GetTexturePointer(_plugin);
            var width = PluginEntry.GetTextureWidth(_plugin);
            var height = PluginEntry.GetTextureHeight(_plugin);

            CheckSharedTextureIsValid(ptr, width, height);
            InitializeSharedTexture(ptr, width, height);
        }

        #endregion

        #region Initializations and Checks

        private void InitializePlugin() {
            if (PluginInitialized) return;

            _plugin = PluginEntry.CreateReceiver(sourceName);
            if (_plugin == IntPtr.Zero) return; // Spout may not be ready.

            PluginInitialized = true;
        }

        private void CheckPluginIsValid() {
            if (!PluginInitialized) return;
            if (PluginEntry.CheckValid(_plugin)) return;

            Util.IssuePluginEvent(PluginEntry.Event.Dispose, _plugin);
            _plugin = IntPtr.Zero;
            PluginInitialized = false;
        }

        private void CheckSharedTextureIsValid(IntPtr ptr, int width, int height) {
            if (!SharedTextureInitialized) return;
            if (width == SharedTexture.width && height == SharedTexture.height && ptr == SharedTexture.GetNativeTexturePtr()) return;
            // Not match: Destroy to get refreshed.
            Util.Destroy(SharedTexture);
            SharedTextureInitialized = false;
        }

        protected void InitializeBlitMaterial() {
            if (BlitMaterialInitialized) return;

            BlitMaterial = new Material(blitShader) {
                hideFlags = HideFlags.DontSave
            };

            BlitMaterialInitialized = true;
        }

        protected void InitializeReceivedTexture() {
            if (ReceivedTextureInitialized) return;
            ReceivedTexture = new RenderTexture(SharedTexture.width, SharedTexture.height, 0) {
                hideFlags = HideFlags.DontSave
            };

            ReceivedTextureInitialized = true;
        }

        private void InitializeSharedTexture(IntPtr ptr, int width, int height) {
            if (SharedTextureInitialized || ptr == IntPtr.Zero) return;

            SharedTexture = Texture2D.CreateExternalTexture(
                width, height, TextureFormat.ARGB32, false, false, ptr
            );
            SharedTexture.hideFlags = HideFlags.DontSave;
            SharedTextureInitialized = true;

            Util.Destroy(ReceivedTexture);
            ReceivedTextureInitialized = false;
        }

        #endregion
    }
}