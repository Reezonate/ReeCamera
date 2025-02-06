using System.Collections;
using System.Text;
using UnityEngine;

namespace ReeCamera {
    internal static class ReeUtils {
        #region PrintHierarchy

        public static IEnumerator PrintFullHierarchy(Transform transform, float delay = 0.001f) {
            var root = transform;
            while (true) {
                if (root.parent == null) break;
                root = root.parent;
            }

            yield return PrintSelfAndChild(root, delay);
        }

        public static IEnumerator PrintSelfAndChild(Transform transform, float delay = 0.001f, int indent = 0) {
            Plugin.Notice(BuildHierarchyString(transform, indent));
            yield return new WaitForSeconds(delay);

            for (var i = 0; i < transform.childCount; i++) {
                var child = transform.GetChild(i);
                yield return PrintSelfAndChild(child, delay, indent + 1);
            }
        }

        public static void PrintSelfAndChildAlt(Transform transform, int indent = 0) {
            Plugin.Notice(BuildHierarchyString(transform, indent));

            for (var i = 0; i < transform.childCount; i++) {
                var child = transform.GetChild(i);
                PrintSelfAndChildAlt(child, indent + 1);
            }
        }

        private static string BuildHierarchyString(Transform transform, int indent) {
            var output = new StringBuilder();

            output.Append(transform.gameObject.activeInHierarchy ? "+" : "-");

            for (var i = 0; i < indent; i++) {
                output.Append(".  ");
            }

            output.Append(transform.name);
            output.Append(" [");

            var components = transform.GetComponents<Component>();
            for (var i = 0; i < components.Length; i++) {
                if (i > 0) output.Append(", ");
                var component = components[i];
                output.Append(component.GetType().FullName);
            }

            output.Append(" ]");
            return output.ToString();
        }

        #endregion
    }
}