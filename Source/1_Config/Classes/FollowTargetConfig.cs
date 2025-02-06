using Newtonsoft.Json;
using UnityEngine;

namespace ReeCamera {
    public struct FollowTargetConfig {
        public OffsetType OffsetType;
        public Vector3 PositionOffset;
        public bool ForceUpright;
        public float PositionalSmoothing;
        public float RotationalSmoothing;

        [JsonConstructor]
        public FollowTargetConfig(int _) {
            OffsetType = OffsetType.Global;
            PositionOffset = new Vector3(0.0f, 0.0f, 0.0f);
            ForceUpright = false;
            PositionalSmoothing = 0.0f;
            RotationalSmoothing = 0.0f;
        }

        public static FollowTargetConfig Default => new FollowTargetConfig(0);
    }
}