using System;
using BeatmapEditor3D;
using HarmonyLib;
using JetBrains.Annotations;

namespace ReeCamera;

[HarmonyPatch(typeof(BeatmapLevelEditorInstaller), "InstallBindings")]
public static class BeatmapEditorInstallerPatch {
    [UsedImplicitly]
    // ReSharper disable once InconsistentNaming
    private static void Postfix(BeatmapLevelEditorInstaller __instance) {
        try {
            OnEditorInstaller.Install(__instance.Container);
        } catch (Exception ex) {
            Plugin.Log.Critical($"---\nBeatmapLevelEditorInstaller exception: {ex.Message}\n{ex.StackTrace}\n---");
        }
    }
}