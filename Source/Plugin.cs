using System.IO;
using IPA;
using IPA.Utilities;
using JetBrains.Annotations;
using ReeCamera.Spout;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace ReeCamera {
    [Plugin(RuntimeOptions.SingleStartInit), UsedImplicitly]
    public class Plugin {
        #region Log

        internal const string ResourcesPath = "ReeCamera._9_Resources";
        internal const string HarmonyId = "Reezonate.ReeCamera";
        internal const string FancyName = "ReeCamera";
        internal const string ModVersion = "0.0.5";

        public static readonly string UserDataDirectory = Path.Combine(UnityGame.UserDataPath, "ReeCamera");
        public static readonly string MainConfigPath = Path.Combine(UnityGame.UserDataPath, "ReeCamera.json");

        internal static void Notice(string message) {
            Log.Notice(message);
        }

        internal static void Warning(string message) {
            Log.Warn(message);
        }

        internal static void Error(string message) {
            Log.Error(message);
        }

        #endregion

        #region Init

        internal static IPALogger Log { get; private set; }

        [Init]
        public Plugin(IPALogger logger, Zenjector zenjector) {
            Log = logger;
            BundleLoader.Initialize();
            zenjector.Install<OnAppInstaller>(Location.App);
            zenjector.Install<OnMenuInstaller>(Location.Menu);
            zenjector.Install<OnGameInstaller>(Location.Singleplayer);
            zenjector.Install<OnGameInstaller>(Location.MultiPlayer);
        }

        #endregion

        #region OnApplicationStart

        [OnStart, UsedImplicitly]
        public void OnApplicationStart() {
            SpoutLoader.LoadPlugin();
            HarmonyHelper.ApplyPatches();
            MainPluginConfig.Load(MainConfigPath);
        }

        #endregion

        #region OnApplicationQuit

        [OnExit, UsedImplicitly]
        public void OnApplicationQuit() { }

        #endregion
    }
}