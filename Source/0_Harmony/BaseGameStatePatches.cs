using HarmonyLib;
using JetBrains.Annotations;

namespace ReeCamera {
    [HarmonyPatch(typeof(SettingsApplicatorSO), nameof(SettingsApplicatorSO.ApplyGameSettings))]
    public static class SettingsApplicatorPatch {
        [UsedImplicitly]
        // ReSharper disable once InconsistentNaming
        private static void Postfix(in BeatSaber.Settings.Settings settings) {
            PluginStateManager.BaseGameSettingOV.SetValue(settings, PluginStateManager.BaseGameSettingOV);
        }
    }
}