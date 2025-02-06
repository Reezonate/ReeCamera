namespace ReeCamera {
    public class MassV1OnSpring {
        public float TargetValue;
        public float CurrentValue;

        public float Elasticity;
        public float Mass;
        public float Drag;

        private float _velocity;

        public void FixedUpdate() {
            AddElasticForce();
            AddDragForce();
        }

        public void Update(float deltaTime) {
            CurrentValue += _velocity * deltaTime;
        }

        private void AddDragForce() {
            const int resolution = 10;

            for (var i = 0; i < resolution; i++) {
                var dragForce = -_velocity * Drag / resolution;
                AddForce(dragForce);
            }
        }

        private void AddElasticForce() {
            var difference = TargetValue - CurrentValue;
            var forceValue = Elasticity * difference;
            AddForce(forceValue);
        }

        public void AddForce(float force) {
            var acceleration = force / Mass;
            _velocity += acceleration;
        }
    }
}