using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using IPA.Utilities;

namespace ReeCamera.Spout {
    internal static class SpoutLoader {
        private static readonly string PluginsDirectory = Path.Combine(UnityGame.InstallPath, @"Beat Saber_Data\Plugins\x86_64\");
        private static readonly string DllPath = Path.Combine(PluginsDirectory, "KlakSpout.dll");
        private const string ResourceName = Plugin.ResourcesPath + ".Plugins.KlakSpout.dll";

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        public static void LoadPlugin() {
            if (!Directory.Exists(PluginsDirectory)) return;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName)) {
                using (var fs = new FileStream(DllPath, FileMode.Create, FileAccess.Write)) {
                    stream.CopyTo(fs);
                }
            }

            var handle = LoadLibrary(DllPath);
            if (handle == IntPtr.Zero) {
                var errorCode = Marshal.GetLastWin32Error();
                Plugin.Error($"Failed to load Spout DLL! Win32 Error Code: {errorCode}");
            } else {
                Plugin.Notice("Spout loaded Successfully!");
            }
        }
    }
}