using Newtonsoft.Json;
using UnityEngine;

namespace ReeCamera {
    public struct MovementConfig {
        public MovementType MovementType;
        public Vector3 InitialPosition;
        public Vector3 InitialRotation;

        [JsonConstructor]
        public MovementConfig(int _) {
            MovementType = MovementType.FollowTarget;
            InitialPosition = new Vector3(0.0f, 1.6f, -1.0f);
            InitialRotation = new Vector3(0.0f, 0.0f, 0.0f);
        }

        public static MovementConfig Default => new MovementConfig(0);
    }
}