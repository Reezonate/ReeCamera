using ReeCamera.Spout;
using UnityEngine;
using UnityEngine.UI;

namespace ReeCamera {
    public class MainCameraController : AbstractCameraController<MainCameraConfig> {
        #region Instantiate

        public static MainCameraController Instantiate(
            Transform parent,
            MainCameraConfig config,
            GameObject cameraPrefab,
            IObservableValue<Rect> screenRectOV,
            IObservableValue<bool> isVisibleOV
        ) {
            var go = Instantiate(cameraPrefab, parent, false);
            var camera = go.GetComponent<Camera>();
            go.SetActive(true);

            var component = go.AddComponent<MainCameraController>();
            component.Construct(config, camera);
            component._screenRectOV = screenRectOV;
            component._isVisibleOV = isVisibleOV;
            return component;
        }

        #endregion

        #region Init / Dispose

        private IObservableValue<Rect> _screenRectOV;
        private IObservableValue<bool> _isVisibleOV;
        private TextureSpoutSender _spoutSender;
        private RawImage _screenImage;

        private void Awake() {
            _spoutSender = gameObject.AddComponent<TextureSpoutSender>();
            _spoutSender.blitShader = BundleLoader.Materials.spoutBlitMaterial.shader;
            _spoutSender.enabled = false;
        }

        protected override void Start() {
            base.Start();
            
            var imageGo = new GameObject("ScreenImage");
            _screenImage = imageGo.AddComponent<RawImage>();
            _screenImage.material = BundleLoader.Materials.screenImageMaterial;
            _screenImage.rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
            _screenImage.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _screenImage.rectTransform.offsetMin = new Vector2(0.0f, 0.0f);
            _screenImage.rectTransform.offsetMax = new Vector2(0.0f, 0.0f);
            _screenImage.enabled = false;

            _screenRectOV.AddStateListener(OnScreenRectChanged, this);
            _isVisibleOV.AddStateListener(OnIsVisibleChanged, this);
            Config.SpoutSettingsOV.AddStateListener(OnSpoutSettingsChanged, this);
            Config.QualitySettingsOV.AddStateListener(OnQualitySettingsChanged, this);
            PluginState.ScreenResolution.AddStateListener(OnScreenResolutionChanged, this);
            PluginState.ScreenCanvasOV.AddStateListener(OnScreenCanvasChanged, this);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            
            Destroy(_screenImage.gameObject);

            _screenRectOV.RemoveStateListener(OnScreenRectChanged);
            _isVisibleOV.RemoveStateListener(OnIsVisibleChanged);
            Config.SpoutSettingsOV.RemoveStateListener(OnSpoutSettingsChanged);
            Config.QualitySettingsOV.RemoveStateListener(OnQualitySettingsChanged);
            PluginState.ScreenResolution.RemoveStateListener(OnScreenResolutionChanged);
            PluginState.ScreenCanvasOV.RemoveStateListener(OnScreenCanvasChanged);

            DisposeOutputTexture();
        }

        protected override void Update() {
            UpdateOutputTextureIfDirty();
            UpdateSpoutIfDirty();
            base.Update();
        }

        #endregion

        #region Events

        private SpoutSettings _spoutSettings;
        private Resolution _screenResolution = new Resolution { width = 1920, height = 1080, refreshRate = 60 };
        private QualitySettings _qualitySettings;
        private Rect _screenRect = new Rect(0, 0, 1, 1);

        private void OnIsVisibleChanged(bool value, ObservableValueState state) {
            _screenImage.gameObject.SetActive(value);
        }

        private void OnScreenCanvasChanged(Canvas value, ObservableValueState state) {
            _screenImage.transform.SetParent(value.transform, false);
        }

        private void OnScreenRectChanged(Rect value, ObservableValueState state) {
            _screenRect = value;
            MarkOutputTextureDirty();
            MarkSpoutDirty();
        }

        private void OnQualitySettingsChanged(QualitySettings value, ObservableValueState state) {
            _qualitySettings = value;
            MarkOutputTextureDirty();
            MarkSpoutDirty();
        }

        private void OnScreenResolutionChanged(Resolution value, ObservableValueState state) {
            _screenResolution = value;
            MarkOutputTextureDirty();
            MarkSpoutDirty();
        }

        private void OnSpoutSettingsChanged(SpoutSettings value, ObservableValueState state) {
            _spoutSettings = value;
            MarkOutputTextureDirty();
            MarkSpoutDirty();
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
        }

        private void InitOutputTexture() {
            if (_outputTextureInitialized) return;

            var pixelRect = new Vector4(_screenRect.x, _screenRect.y, _screenRect.width, _screenRect.height);
            CompositionManagerSO.AdjustRectangle(pixelRect, _screenResolution.width, _screenResolution.height, out var rect, out var textureWidth, out var textureHeight);
            _screenImage.rectTransform.anchorMin = new Vector2(rect.x, rect.y);
            _screenImage.rectTransform.anchorMax = new Vector2(rect.x + rect.z, rect.y + rect.w);

            if (_spoutSettings.Enabled) {
                textureWidth = Mathf.RoundToInt(_spoutSettings.Width * _qualitySettings.RenderScale);
                textureHeight = Mathf.RoundToInt(_spoutSettings.Height * _qualitySettings.RenderScale);
            } else {
                textureWidth = Mathf.RoundToInt(textureWidth * _qualitySettings.RenderScale);
                textureHeight = Mathf.RoundToInt(textureHeight * _qualitySettings.RenderScale);
            }

            if (textureWidth < 1) textureWidth = 1;
            if (textureHeight < 1) textureHeight = 1;

            _outputTexture = new RenderTexture(textureWidth, textureHeight, 32, RenderTextureFormat.ARGB32) {
                filterMode = FilterMode.Bilinear,
                antiAliasing = _qualitySettings.AntiAliasing
            };

            _outputTexture.Create();

            _screenImage.texture = _outputTexture;
            _screenImage.enabled = true;
            SetTargetTexture(_outputTexture);

            _outputTextureInitialized = true;
        }

        private void DisposeOutputTexture() {
            if (!_outputTextureInitialized) return;

            _outputTexture.Release();
            _outputTexture = null;
            _outputTextureInitialized = false;
            _screenImage.texture = null;
            _screenImage.enabled = false;
        }

        #endregion
    }
}