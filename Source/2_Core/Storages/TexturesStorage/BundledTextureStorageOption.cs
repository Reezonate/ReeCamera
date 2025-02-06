using UnityEngine;

namespace ReeSabers.Storages {
    public class BundledTextureStorageOption : IStorageOption<Texture2D> {
        #region MyRegion

        private readonly Texture2D _texture;

        public BundledTextureStorageOption(string optionId, string displayName, Texture2D texture) {
            _texture = texture;
            Id = optionId;
            DisplayName = displayName;
        }

        #endregion

        #region IStorageOption

        public string Id { get; }

        public string DisplayName { get; }

        public bool TryLoad(out Texture2D result, out string failReason) {
            result = _texture;
            failReason = "";
            return true;
        }

        #endregion
    }
}