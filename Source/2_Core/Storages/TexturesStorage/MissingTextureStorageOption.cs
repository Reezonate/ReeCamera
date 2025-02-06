using ReeCamera;
using UnityEngine;

namespace ReeSabers.Storages {
    public class MissingTextureStorageOption : IStorageOption<Texture2D> {
        #region MyRegion

        public MissingTextureStorageOption(string optionId) {
            Id = optionId;
            DisplayName = $"<color=#FF8888>Missing ({optionId})</color>";
        }

        #endregion

        #region IStorageOption

        public string Id { get; }

        public string DisplayName { get; }

        public bool TryLoad(out Texture2D result, out string failReason) {
            result = BundleLoader.Textures.missingTexture;
            failReason = "";
            return true;
        }

        #endregion
    }
}