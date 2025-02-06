using ReeCamera.Spout;
using UnityEngine;
using UnityEngine.UI;

namespace ReeCamera {
    public class OutputController : MonoBehaviour {
        #region Init / Dispose

        private TextureSpoutSender _spoutSender;
        private Canvas _screenCanvas;
        private RawImage _screenImage;

        private void Awake() {
            _spoutSender = gameObject.AddComponent<TextureSpoutSender>();
            _spoutSender.blitShader = BundleLoader.Materials.spoutBlitMaterial.shader;
            _spoutSender.enabled = false;

            var canvasGo = new GameObject("ScreenCanvas");
            _screenCanvas = canvasGo.AddComponent<Canvas>();
            _screenCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _screenCanvas.enabled = false;

            var imageGo = new GameObject("ScreenImage");
            imageGo.transform.SetParent(canvasGo.transform, false);
            _screenImage = imageGo.AddComponent<RawImage>();
            _screenImage.material = BundleLoader.Materials.screenImageMaterial;
            _screenImage.rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
            _screenImage.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _screenImage.rectTransform.offsetMin = new Vector2(0.0f, 0.0f);
            _screenImage.rectTransform.offsetMax = new Vector2(0.0f, 0.0f);

            DontDestroyOnLoad(canvasGo);
        }

        private void Start() {
            MainPluginConfig.Instance.Output.SpoutSettingsOV.AddStateListener(OnSpoutSettingsChanged, this);
            MainPluginConfig.Instance.Output.AntiAliasingOV.AddStateListener(OnAntiAliasingChanged, this);
            PluginState.ScreenResolution.AddStateListener(OnScreenResolutionChanged, this);
        }

        private void OnDestroy() {
            MainPluginConfig.Instance.Output.SpoutSettingsOV.RemoveStateListener(OnSpoutSettingsChanged);
            MainPluginConfig.Instance.Output.AntiAliasingOV.RemoveStateListener(OnAntiAliasingChanged);
            PluginState.ScreenResolution.RemoveStateListener(OnScreenResolutionChanged);

            if (_screenCanvas?.gameObject != null) {
                Destroy(_screenCanvas.gameObject);
            }

            DisposeOutputTexture();
        }

        private void Update() {
            UpdateOutputTextureIfDirty();
            UpdateScreenIfDirty();
            UpdateSpoutIfDirty();
        }

        #endregion

        #region Events

        private SpoutSettings _spoutSettings;
        private Resolution _screenResolution = new Resolution { width = 1920, height = 1080, refreshRate = 60 };
        private int _antiAliasing = 1;

        private void OnAntiAliasingChanged(int value, ObservableValueState state) {
            _antiAliasing = value;
            MarkOutputTextureDirty();
            MarkScreenDirty();
            MarkSpoutDirty();
        }

        private void OnScreenResolutionChanged(Resolution value, ObservableValueState state) {
            _screenResolution = value;
            MarkOutputTextureDirty();
            MarkScreenDirty();
            MarkSpoutDirty();
        }

        private void OnSpoutSettingsChanged(SpoutSettings value, ObservableValueState state) {
            _spoutSettings = value;
            MarkOutputTextureDirty();
            MarkSpoutDirty();
        }

        #endregion

        #region Screen

        private bool _screenDirty;
        private bool _screenInitialized;

        private void MarkScreenDirty() {
            _screenDirty = true;
        }

        private void UpdateScreenIfDirty() {
            if (!_screenDirty) return;
            _screenDirty = false;

            DisposeScreen();
            InitScreen();
        }

        private void InitScreen() {
            if (_screenInitialized) return;

            _screenImage.texture = _outputTexture;
            _screenCanvas.enabled = true;

            _screenInitialized = true;
        }

        private void DisposeScreen() {
            if (!_screenInitialized) return;

            _screenImage.texture = null;
            _screenCanvas.enabled = false;

            _screenInitialized = false;
        }

        #endregion

        #region Spout

        private bool _spoutTextureDirty;
        private bool _spoutInitialized;

        private void MarkSpoutDirty() {
            _spoutTextureDirty = true;
        }

        private void UpdateSpoutIfDirty() {
            if (!_spoutTextureDirty) return;
            _spoutTextureDirty = false;

            DisposeSpout();

            if (_spoutSettings.Enabled) {
                InitSpout();
            }
        }

        private void InitSpout() {
            if (_spoutInitialized) return;
            _spoutSender.channelName = _spoutSettings.ChannelName;
            _spoutSender.sourceTexture = _outputTexture;
            _spoutSender.enabled = true;
            _spoutInitialized = true;
        }

        private void DisposeSpout() {
            if (!_spoutInitialized) return;

            _spoutSender.sourceTexture = null;
            _spoutSender.enabled = false;
            _spoutInitialized = false;
        }

        #endregion

        #region Texture

        private RenderTexture _outputTexture;
        private bool _outputTextureDirty;
        private bool _outputTextureInitialized;

        private void MarkOutputTextureDirty() {
            _outputTextureDirty = true;
        }

        private void UpdateOutputTextureIfDirty() {
            if (!_outputTextureDirty) return;
            _outputTextureDirty = false;

            DisposeOutputTexture();
            InitOutputTexture();

            PluginState.OutputTexture.SetValue(_outputTexture, this);
        }

        private void InitOutputTexture() {
            if (_outputTextureInitialized) return;

            if (_spoutSettings.Enabled) {
                _outputTexture = new RenderTexture(_spoutSettings.Width, _spoutSettings.Height, 32, RenderTextureFormat.ARGB32) {
                    filterMode = FilterMode.Bilinear,
                    antiAliasing = _antiAliasing
                };
            } else {
                _outputTexture = new RenderTexture(_screenResolution.width, _screenResolution.height, 32, RenderTextureFormat.ARGB32) {
                    filterMode = FilterMode.Bilinear,
                    antiAliasing = _antiAliasing
                };
            }

            _outputTexture.Create();
            _outputTextureInitialized = true;
        }

        private void DisposeOutputTexture() {
            if (!_outputTextureInitialized) return;

            _outputTexture.Release();
            _outputTexture = null;
            _outputTextureInitialized = false;
        }

        #endregion
    }
}