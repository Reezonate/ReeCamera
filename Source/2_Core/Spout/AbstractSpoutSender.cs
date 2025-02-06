using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ReeCamera.Spout {
    public abstract class AbstractSpoutSender : MonoBehaviour {
        public Shader blitShader;
        public string channelName = "SpoutSender";

        #region Format options

        [SerializeField]
        private bool alphaSupport;

        public bool AlphaSupport {
            get => alphaSupport;
            set => alphaSupport = value;
        }

        #endregion

        #region Private members

        private IntPtr _plugin;
        private Texture2D _sharedTexture;
        private Material _blitMaterial;
        private bool _sharedTextureInitialized;
        private bool _blitMaterialInitialized;
        private bool _pluginInitialized;

        private static readonly int ClearAlpha = Shader.PropertyToID("_ClearAlpha");

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int GetLastError();

        private void SendRenderTexture(Texture source) {
            InitializePlugin(source.width, source.height);
            if (!_pluginInitialized) return;
            InitializeSharedTexture();
            if (!_sharedTextureInitialized) return;
            InitializeBlitMaterial();

            // Blit shader parameters
            _blitMaterial.SetFloat(ClearAlpha, alphaSupport ? 0 : 1);

            var tempRT = RenderTexture.GetTemporary(_sharedTexture.width, _sharedTexture.height);
            Graphics.Blit(source, tempRT, _blitMaterial, 0);
            Graphics.CopyTexture(tempRT, _sharedTexture);
            RenderTexture.ReleaseTemporary(tempRT);
        }

        private void InitializePlugin(int width, int height) {
            if (_pluginInitialized) return;

            _plugin = PluginEntry.CreateSender(channelName, width, height);
            if (_plugin == IntPtr.Zero) {
                int errorCode = GetLastError();
                Plugin.Log.Notice($"Error: {errorCode}");
                return; // Spout may not be ready.
            }

            _pluginInitialized = true;
        }

        private void InitializeSharedTexture() {
            if (_sharedTextureInitialized) return;

            var ptr = PluginEntry.GetTexturePointer(_plugin);
            if (ptr == IntPtr.Zero) return;

            _sharedTexture = Texture2D.CreateExternalTexture(
                PluginEntry.GetTextureWidth(_plugin),
                PluginEntry.GetTextureHeight(_plugin),
                TextureFormat.ARGB32, false, false, ptr
            );
            _sharedTexture.hideFlags = HideFlags.DontSave;
            _sharedTextureInitialized = true;
        }

        private void InitializeBlitMaterial() {
            if (_blitMaterialInitialized) return;

            _blitMaterial = new Material(blitShader) {
                hideFlags = HideFlags.DontSave
            };
            _blitMaterialInitialized = true;
        }

        #endregion

        #region Protected members

        protected void SendTextureMode(RenderTexture texture) {
            SendRenderTexture(texture);
        }

        protected void SendCameraMode(RenderTexture source, RenderTexture destination) {
            SendRenderTexture(source);
            Graphics.Blit(source, destination);
        }

        #endregion

        #region MonoBehaviour implementation

        protected virtual void Update() {
            if (_pluginInitialized) {
                Util.IssuePluginEvent(PluginEntry.Event.Update, _plugin);
            }
        }

        protected virtual void OnDisable() {
            if (_pluginInitialized) {
                Util.IssuePluginEvent(PluginEntry.Event.Dispose, _plugin);
                _plugin = IntPtr.Zero;
                _pluginInitialized = false;
            }

            Util.Destroy(_sharedTexture);
            _sharedTextureInitialized = false;
        }

        protected virtual void OnDestroy() {
            Util.Destroy(_blitMaterial);
        }

        #endregion
    }
}