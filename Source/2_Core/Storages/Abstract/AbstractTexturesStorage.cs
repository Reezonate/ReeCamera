using System;
using System.IO;
using UnityEngine;

namespace ReeCamera {
    public abstract class AbstractTexturesStorage<TI> : AbstractFilesStorage<Texture2D, TI> where TI : AbstractTexturesStorage<TI> {
        #region LoadOrError

        public Texture2D LoadOrError(string optionId) {
            if (!TryGetOption(optionId, out var option)) {
                Plugin.Error($"Unable to find texture! ID: {optionId}");
                return BundleLoader.Textures.missingTexture;
            }

            if (option.TryLoad(out var texture, out var failReason)) return texture;

            Plugin.Error($"Unable to load texture! ID: {optionId} reason: {failReason}");
            return BundleLoader.Textures.missingTexture;
        }

        #endregion

        #region TryLoadItem

        protected override bool TryLoadItem(string absolutePath, out Texture2D result, out string failReason) {
            try {
                var bytes = File.ReadAllBytes(absolutePath);
                result = CreateTexture();
                result.LoadImage(bytes);
                OnTextureWasLoaded(result);
                failReason = "";
                return true;
            } catch (Exception ex) {
                result = default;
                failReason = ex.Message;
                return false;
            }
        }

        protected virtual Texture2D CreateTexture() {
            return new Texture2D(1, 1);
        }

        protected virtual void OnTextureWasLoaded(Texture2D texture) { }

        #endregion

        #region TryRemoveItem

        protected override bool TryRemoveItem(string absolutePath, out string failReason) {
            try {
                File.Delete(absolutePath);
                MarkOptionsDirty();
                failReason = "";
                return true;
            } catch (Exception ex) {
                failReason = ex.Message;
                return false;
            }
        }

        #endregion
    }
}