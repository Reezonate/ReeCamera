using UnityEngine;

namespace ReeCamera {
    public class MassV3OnSpring {
        public Vector3 TargetValue;
        public Vector3 CurrentValue;
        public float Elasticity;
        public float Mass;
        public float Drag;

        private Vector3 _velocity;

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
            var forceNormal = difference.normalized;
            var dl = difference.magnitude;
            var forceValue = Elasticity * dl;
            AddForce(forceNormal * forceValue);
        }

        public void AddForce(Vector3 force) {
            var acceleration = force / Mass;
            _velocity += acceleration;
        }
    }
}