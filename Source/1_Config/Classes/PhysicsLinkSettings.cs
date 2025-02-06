using Newtonsoft.Json;

namespace ReeCamera {
    public struct PhysicsLinkSettings {
        public bool UsePhysics;
        public float LookAtOffset;
        public float CameraMass;
        public float PositionSpring;
        public float DirectionSpring;
        public float RotZSpring;
        public float PositionDrag;
        public float DirectionDrag;
        public float RotZDrag;

        [JsonConstructor]
        public PhysicsLinkSettings(int _) {
            UsePhysics = false;
            LookAtOffset = 2.0f;
            CameraMass = 1f;
            PositionSpring = 10.0f;
            DirectionSpring = 10.0f;
            RotZSpring = 10.0f;
            PositionDrag = 0.6f;
            DirectionDrag = 0.6f;
            RotZDrag = 0.6f;
        }

        public static PhysicsLinkSettings Default => new PhysicsLinkSettings(0);
    }
}