// KlakSpout - Spout video frame sharing plugin for Unity
// https://github.com/keijiro/KlakSpout

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ReeCamera.Spout {
    // Internal utilities
    internal static class Util {
        // Scan available Spout sources and return their names via a newly
        // allocated string array.
        public static string[] GetSourceNames() {
            var count = PluginEntry.ScanSharedObjects();
            var names = new string [count];
            for (var i = 0; i < count; i++) {
                names[i] = PluginEntry.GetSharedObjectNameString(i);
            }

            return names;
        }

        // Scan available Spout sources and store their names into the given
        // collection object.
        public static void GetSourceNames(ICollection<string> store) {
            store.Clear();
            var count = PluginEntry.ScanSharedObjects();
            for (var i = 0; i < count; i++) {
                store.Add(PluginEntry.GetSharedObjectNameString(i));
            }
        }

        internal static void Destroy(Object obj) {
            if (obj == null) return;

            if (Application.isPlaying) {
                Object.Destroy(obj);
            } else {
                Object.DestroyImmediate(obj);
            }
        }

        private static CommandBuffer _commandBuffer;

        internal static void IssuePluginEvent(PluginEntry.Event pluginEvent, System.IntPtr ptr) {
            if (_commandBuffer == null) _commandBuffer = new CommandBuffer();

            _commandBuffer.IssuePluginEventAndData(
                PluginEntry.GetRenderEventFunc(), (int)pluginEvent, ptr
            );

            Graphics.ExecuteCommandBuffer(_commandBuffer);

            _commandBuffer.Clear();
        }
    }
}