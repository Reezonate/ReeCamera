using System.IO;
using JetBrains.Annotations;
using ReeCamera;
using UnityEngine;

namespace ReeSabers.Storages {
    [UsedImplicitly]
    public class CustomTexturesStorage : AbstractTexturesStorage<CustomTexturesStorage> {
        #region AbstractTexturesStorage

        protected override string DirectoryPath { get; } = Path.Combine(Plugin.UserDataDirectory, "CustomTextures");

        #endregion

        #region CreateTexture

        private static BundledTextureStorageOption NoneOption => new BundledTextureStorageOption("", "None", BundleLoader.Textures.whitePixel);

        protected override void FillOptionsList() {
            OptionsList.Add(NoneOption);
            base.FillOptionsList();
        }

        protected override Texture2D CreateTexture() {
            return new Texture2D(1, 1, TextureFormat.RGBAFloat, false) {
                wrapMode = TextureWrapMode.Clamp
            };
        }

        #endregion
    }
}