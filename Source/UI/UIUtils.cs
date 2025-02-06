using HMUI;

namespace ReeCamera.UI {
    internal static class UIUtils {
        #region Skew

        public static void ApplySkew(ImageView image, float skew = 0.18f) {
            if (image == null) return;
            image._skew = skew;
        }

        #endregion
    }
}