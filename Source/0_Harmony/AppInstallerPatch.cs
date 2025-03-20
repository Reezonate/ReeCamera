using HarmonyLib;
using JetBrains.Annotations;

namespace ReeCamera {
    [HarmonyPatch(typeof(PCAppInit), "InstallBindings")]
    public static class AppInstallerPatch {
        [UsedImplicitly]
        // ReSharper disable once InconsistentNaming
        private static void Postfix(PCAppInit __instance) {
            PluginState.MainSettingsModelOV.SetValue(__instance._mainSettingsModel, __instance);
        }
    }
}