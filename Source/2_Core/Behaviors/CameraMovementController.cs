using UnityEngine;

namespace ReeCamera {
    [RequireComponent(typeof(Camera))]
    public class CameraMovementController : MonoBehaviour {
        #region Instantiate

        public static CameraMovementController Instantiate(GameObject gameObject, AbstractCameraConfig config) {
            var component = gameObject.AddComponent<CameraMovementController>();
            component.Construct(config);
            return component;
        }

        #endregion

        #region Init / Dispose

        public AbstractCameraConfig Config { get; private set; }

        private void Construct(AbstractCameraConfig config) {
            Config = config;
        }

        private void Start() {
            Config.MovementConfigOV.AddStateListener(OnMovementConfigChanged, this);
            // Config.PhysicsLinkSettingsOV.AddStateListener(OnPhysicsLinkSettingChanged, this);
            PluginState.NoteWasCutEvent += OnNoteWasCut;
        }

        private void OnDestroy() {
            Config.MovementConfigOV.RemoveStateListener(OnMovementConfigChanged);
            // Config.PhysicsLinkSettingsOV.RemoveStateListener(OnPhysicsLinkSettingChanged);
            PluginState.NoteWasCutEvent -= OnNoteWasCut;
        }

        #endregion

        #region Events

        private void OnNoteWasCut() {
            PositionSpring.AddForce(Vector3.back * 1);
        }

        private void FixedUpdate() {
            FixedUpdatePhysicsLink();
        }

        private void OnPreRender() {
            UpdateMovement();
        }

        #endregion

        #region Movement

        private MovementConfig _movementConfig = MovementConfig.Default;
        private int _resetFrames;

        private void OnMovementConfigChanged(MovementConfig value, ObservableValueState state) {
            _movementConfig = value;
            _resetFrames = 5;
        }

        private void UpdateMovement() {
            if (_resetFrames > 0) {
                ResetTarget();
                ResetPhysicsPose();
                _resetFrames -= 1;
            }

            UpdateTargetPose();

            switch (_movementConfig.MovementType) {
                case MovementType.FollowTarget: {
                    if (_physicsLinkSettings.UsePhysics) {
                        UpdatePhysicsPose();
                        ApplyPose(_physicsPose);
                    } else {
                        ApplyPose(_targetPose);
                    }

                    break;
                }
                case MovementType.Static:
                default: {
                    ApplyPose(_targetPose);
                    break;
                }
            }
        }

        private void ApplyPose(in ReeTransform pose) {
            transform.SetLocalPositionAndRotation(pose.Position, pose.Rotation);
        }

        #endregion

        #region Target

        private ReeTransform _targetPose;
        private ReeTransform _tempSmoothPose;

        public void ResetTarget() {
            GetTargetPoses(out _tempSmoothPose, out var localOffset);
            _targetPose = _tempSmoothPose;
            _targetPose.Position += _tempSmoothPose.Rotation * localOffset.Position;
        }

        private void GetTargetPoses(out ReeTransform smoothPose, out ReeTransform localOffset) {
            switch (_movementConfig.MovementType) {
                case MovementType.Static: {
                    smoothPose = ReeTransform.Identity;
                    localOffset = ReeTransform.Identity;
                    break;
                }
                case MovementType.FollowTarget:
                default: {
                    var parentPose = ReeTransform.FromTransform(transform.parent);
                    var targetWorldPose = PluginState.FirstPersonPoseOV.Value;

                    smoothPose = new ReeTransform(
                        parentPose.WorldToLocalPosition(targetWorldPose.Position),
                        parentPose.WorldToLocalRotation(targetWorldPose.Rotation)
                    );
                    break;
                }
            }

            switch (_movementConfig.OffsetType) {
                case OffsetType.Local: {
                    smoothPose.Rotation *= Quaternion.Euler(_movementConfig.RotationOffset);
                    localOffset = new ReeTransform(
                        _movementConfig.PositionOffset,
                        Quaternion.identity
                    );
                    break;
                }
                case OffsetType.Global:
                default: {
                    smoothPose.Position += _movementConfig.PositionOffset;
                    smoothPose.Rotation *= Quaternion.Euler(_movementConfig.RotationOffset);
                    localOffset = ReeTransform.Identity;
                    break;
                }
            }

            if (_movementConfig.ForceUpright) {
                smoothPose.Rotation = Quaternion.LookRotation(smoothPose.Forward, Vector3.up);
            }
        }

        private void UpdateTargetPose() {
            GetTargetPoses(out var smoothPose, out var localOffset);

            _tempSmoothPose.Position = _movementConfig.PositionalSmoothing > 0
                ? Vector3.Lerp(_tempSmoothPose.Position, smoothPose.Position, Time.deltaTime * _movementConfig.PositionalSmoothing)
                : smoothPose.Position;
            _tempSmoothPose.Rotation = _movementConfig.RotationalSmoothing > 0
                ? Quaternion.Lerp(_tempSmoothPose.Rotation, smoothPose.Rotation, Time.deltaTime * _movementConfig.RotationalSmoothing)
                : smoothPose.Rotation;
            
            _targetPose = _tempSmoothPose;
            _targetPose.Position += _tempSmoothPose.Rotation * localOffset.Position;
        }

        #endregion

        #region PhysicsLink

        public readonly MassV3OnSpring PositionSpring = new MassV3OnSpring();
        public readonly MassV3OnSpring LookAtSpring = new MassV3OnSpring();
        public readonly MassV1OnSpring RotZSpring = new MassV1OnSpring();

        private PhysicsLinkSettings _physicsLinkSettings = PhysicsLinkSettings.Default;
        private ReeTransform _physicsPose = ReeTransform.Identity;

        private void OnPhysicsLinkSettingChanged(PhysicsLinkSettings value, ObservableValueState state) {
            _physicsLinkSettings = value;

            PositionSpring.Mass = value.CameraMass;
            PositionSpring.Drag = value.PositionDrag;
            PositionSpring.Elasticity = value.PositionSpring;

            LookAtSpring.Mass = value.CameraMass;
            LookAtSpring.Drag = value.DirectionDrag;
            LookAtSpring.Elasticity = value.DirectionSpring;

            RotZSpring.Mass = value.CameraMass;
            RotZSpring.Drag = value.RotZDrag;
            RotZSpring.Elasticity = value.RotZSpring;
        }

        private void ResetPhysicsPose() {
            GetPhysicsLinkTargetValues(out var position, out var lookAt, out var rotZ);
            PositionSpring.CurrentValue = position;
            LookAtSpring.CurrentValue = lookAt;
            RotZSpring.CurrentValue = rotZ;
            RecalculatePhysicsPose();
        }

        private void GetPhysicsLinkTargetValues(out Vector3 position, out Vector3 lookAt, out float rotZ) {
            position = _targetPose.Position;
            lookAt = _targetPose.Position + _targetPose.Forward * _physicsLinkSettings.LookAtOffset;

            var localUp = _targetPose.WorldToLocalDirection(Vector3.up);
            rotZ = Mathf.Atan2(localUp.x, localUp.y) * Mathf.Rad2Deg;
        }

        private void FixedUpdatePhysicsLink() {
            GetPhysicsLinkTargetValues(out var position, out var lookAt, out var rotZ);

            PositionSpring.TargetValue = position;
            PositionSpring.FixedUpdate();

            LookAtSpring.TargetValue = lookAt;
            LookAtSpring.FixedUpdate();

            RotZSpring.TargetValue = rotZ;
            RotZSpring.FixedUpdate();
        }

        private void UpdatePhysicsPose() {
            PositionSpring.Update(Time.deltaTime);
            LookAtSpring.Update(Time.deltaTime);
            RotZSpring.Update(Time.deltaTime);
            RecalculatePhysicsPose();
        }

        private void RecalculatePhysicsPose() {
            _physicsPose.Position = PositionSpring.CurrentValue;
            _physicsPose.Rotation = Quaternion.Euler(0, 0, RotZSpring.CurrentValue) * Quaternion.LookRotation(LookAtSpring.CurrentValue - PositionSpring.CurrentValue);
        }

        #endregion
    }
}