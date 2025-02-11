using Newtonsoft.Json;
using UnityEngine;

namespace ReeCamera {
    public struct MovementConfig {
        public MovementType MovementType;
        public OffsetType OffsetType;
        public Vector3 PositionOffset;
        public Vector3 RotationOffset;
        public bool ForceUpright;
        public float PositionalSmoothing;
        public float RotationalSmoothing;

        [JsonConstructor]
        public MovementConfig(int _) {
            MovementType = MovementType.FollowTarget;
            OffsetType = OffsetType.Global;
            PositionOffset = new Vector3(0.0f, 0.0f, 0.0f);
            RotationOffset = new Vector3(0.0f, 0.0f, 0.0f);
            ForceUpright = false;
            PositionalSmoothing = 0.0f;
            RotationalSmoothing = 0.0f;
        }

        public static MovementConfig Default => new MovementConfig(0);
    }
}