using UnityEngine;

namespace ReeSabers.Storages {
    public class PixelTextureStorageOption : IStorageOption<Texture2D> {
        #region MyRegion

        private readonly Color _color;

        public PixelTextureStorageOption(string optionId, string displayName, Color color) {
            _color = color;
            Id = optionId;
            DisplayName = displayName;
        }

        #endregion

        #region Texture

        private Texture2D _texture;
        private bool _initialized;

        private void LazyInit() {
            if (_initialized) return;
            _texture = new Texture2D(1, 1);
            _texture.SetPixel(0, 0, _color);
            _texture.Apply();
            _initialized = true;
        }

        #endregion

        #region IStorageOption

        public string Id { get; }

        public string DisplayName { get; }

        public bool TryLoad(out Texture2D result, out string failReason) {
            LazyInit();
            result = _texture;
            failReason = "";
            return true;
        }

        #endregion
    }
}