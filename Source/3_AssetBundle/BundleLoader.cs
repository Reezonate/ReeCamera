using System;
using System.Reflection;
using UnityEngine;

namespace ReeCamera {
    public static class BundleLoader {
        #region Initialize

        private const string BundleName = Plugin.ResourcesPath + ".AssetBundles.asset_bundle_2019";
        private static bool _ready;

        public static void Initialize() {
            if (_ready) return;

            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(BundleName);
            var localAssetBundle = AssetBundle.LoadFromStream(stream);

            if (localAssetBundle == null) {
                throw new Exception("AssetBundle load error!");
            }

            LoadAssets(localAssetBundle);

            localAssetBundle.Unload(false);
            _ready = true;
        }

        #endregion

        #region Assets

        public static BundledObjects Objects;
        public static BundledTextures Textures;
        public static BundledMaterials Materials;
        public static CompositionManagerSO CompositionManager;

        private static void LoadAssets(AssetBundle assetBundle) {
            Objects = assetBundle.LoadAsset<BundledObjects>("BundledObjects");
            Textures = assetBundle.LoadAsset<BundledTextures>("BundledTextures");
            Materials = assetBundle.LoadAsset<BundledMaterials>("BundledMaterials");
            CompositionManager = assetBundle.LoadAsset<CompositionManagerSO>("CompositionManager");
        }

        #endregion
    }
}