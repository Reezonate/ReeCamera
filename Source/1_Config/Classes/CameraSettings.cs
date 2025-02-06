using Newtonsoft.Json;

namespace ReeCamera {
    public struct CameraSettings {
        public bool IgnoreCameraUtils;
        public float FieldOfView;
        public float NearClipPlane;
        public float FarClipPlane;
        public bool Orthographic;
        public float OrthographicSize;

        [JsonConstructor]
        public CameraSettings(int _) {
            IgnoreCameraUtils = false;
            FieldOfView = 90.0f;
            NearClipPlane = 0.3f;
            FarClipPlane = 1000f;
            Orthographic = false;
            OrthographicSize = 2.0f;
        }

        public static CameraSettings Default => new CameraSettings(0);
    }
}