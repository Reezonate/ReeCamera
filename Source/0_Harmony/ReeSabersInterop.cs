using System.Reflection;
using HarmonyLib;
using IPA.Loader;

namespace ReeCamera {
    internal static class ReeSabersInterop {
        #region Static

        public static bool SuppressReeSabersFramerateManager = false;

        private static PluginMetadata _pluginMetadata;
        private static object _framerateOV;
        private static MethodInfo _setFramerateMethod;

        static ReeSabersInterop() {
            _pluginMetadata = PluginManager.GetPlugin("ReeSabers");
            if (_pluginMetadata == null) return;

            var stateType = _pluginMetadata.Assembly.GetType("ReeSabers.PluginState");
            if (stateType == null) return;

            var framerateOVField = stateType.GetField("TargetFps", BindingFlags.Public | BindingFlags.Static);
            if (framerateOVField == null) return;

            _setFramerateMethod = framerateOVField.FieldType.GetMethod("SetValue", [typeof(int), typeof(object)]);
            if (_setFramerateMethod == null) return;

            _framerateOV = framerateOVField.GetValue(null);
        }

        public static void SetTargetFramerate(int value) {
            if (_pluginMetadata == null) return;
            
            const int minFPS = 1;
            if (value < minFPS) value = minFPS;
            _setFramerateMethod.Invoke(_framerateOV, [value, _framerateOV]);
        }

        #endregion

        #region Patches

        public static void ApplyPatches(Harmony harmony) {
            if (_pluginMetadata == null) return;

            var type = _pluginMetadata.Assembly.GetType("ReeSabers.FramerateManager");
            if (type == null) return;

            var originalMethod = type.GetMethod("UpdateIfDirty", BindingFlags.NonPublic | BindingFlags.Instance);
            if (originalMethod == null) return;

            var prefix = typeof(ReeSabersInterop).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(originalMethod, new HarmonyMethod(prefix));
        }

        private static bool Prefix() {
            return !SuppressReeSabersFramerateManager;
        }

        #endregion
    }
}