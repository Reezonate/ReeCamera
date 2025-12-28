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
        
        public bool PositionCompensation;
        public int PositionCompensationFrames;
        public Vector3 PositionCompensationTarget;
        
        public bool RotationCompensation;
        public int RotationCompensationFrames;
        public Vector3 RotationCompensationTarget;

        [JsonConstructor]
        public MovementConfig(int _) {
            MovementType = MovementType.FollowTarget;
            OffsetType = OffsetType.Global;
            PositionOffset = new Vector3(0.0f, 0.0f, 0.0f);
            RotationOffset = new Vector3(0.0f, 0.0f, 0.0f);
            ForceUpright = false;
            PositionalSmoothing = 0.0f;
            RotationalSmoothing = 0.0f;

            PositionCompensation = false;
            PositionCompensationFrames = 60;
            PositionCompensationTarget = new Vector3(0, 1.75f, 0);

            RotationCompensation = false;
            RotationCompensationFrames = 60;
            RotationCompensationTarget = new Vector3(10, 0, 0);
        }

        public static MovementConfig Default => new MovementConfig(0);
    }
}