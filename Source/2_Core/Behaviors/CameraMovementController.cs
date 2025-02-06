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
            Config.FollowTargetConfigOV.AddStateListener(OnTargetSettingsChanged, this);
            // Config.PhysicsLinkSettingsOV.AddStateListener(OnPhysicsLinkSettingChanged, this);
            PluginState.NoteWasCutEvent += OnNoteWasCut;
        }

        private void OnDestroy() {
            Config.MovementConfigOV.RemoveStateListener(OnMovementConfigChanged);
            Config.FollowTargetConfigOV.RemoveStateListener(OnTargetSettingsChanged);
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
        private ReeTransform _initialPose = ReeTransform.Identity;
        private int _resetFrames;

        private void OnMovementConfigChanged(MovementConfig value, ObservableValueState state) {
            _movementConfig = value;
            _initialPose = new ReeTransform(value.InitialPosition, Quaternion.Euler(value.InitialRotation));
            _resetFrames = 5;
        }

        private void UpdateMovement() {
            if (_resetFrames > 0) {
                ApplyPose(_initialPose);
                ResetTarget();
                ResetPhysicsPose();
                _resetFrames -= 1;
            }

            switch (_movementConfig.MovementType) {
                case MovementType.FollowTarget: {
                    UpdateTargetPose();

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
                    ApplyPose(_initialPose);
                    break;
                }
            }
        }

        private void ApplyPose(in ReeTransform pose) {
            transform.SetLocalPositionAndRotation(pose.Position, pose.Rotation);
        }

        #endregion

        #region Target

        private FollowTargetConfig _targetSettings = FollowTargetConfig.Default;
        private ReeTransform _targetPose;

        private void OnTargetSettingsChanged(FollowTargetConfig value, ObservableValueState state) {
            _targetSettings = value;
        }

        public void ResetTarget() {
            _targetPose = GetTargetPose();
        }

        private ReeTransform GetTargetPose() {
            var parentPose = ReeTransform.FromTransform(transform.parent);
            var targetWorldPose = PluginState.FirstPersonPoseOV.Value;

            var localPose = new ReeTransform(
                parentPose.WorldToLocalPosition(targetWorldPose.Position),
                parentPose.WorldToLocalRotation(targetWorldPose.Rotation)
            );

            switch (_targetSettings.OffsetType) {
                case OffsetType.Local: {
                    localPose.Position += localPose.Rotation * _targetSettings.PositionOffset;
                    break;
                }
                case OffsetType.Global:
                default: {
                    localPose.Position += _targetSettings.PositionOffset;
                    break;
                }
            }


            if (_targetSettings.ForceUpright) {
                localPose.Rotation = Quaternion.LookRotation(localPose.Forward, Vector3.up);
            }

            return localPose;
        }

        private void UpdateTargetPose() {
            var tmp = GetTargetPose();

            _targetPose.Position = _targetSettings.PositionalSmoothing > 0
                ? Vector3.Lerp(_targetPose.Position, tmp.Position, Time.deltaTime * _targetSettings.PositionalSmoothing)
                : tmp.Position;

            _targetPose.Rotation = _targetSettings.RotationalSmoothing > 0
                ? Quaternion.Lerp(_targetPose.Rotation, tmp.Rotation, Time.deltaTime * _targetSettings.RotationalSmoothing)
                : tmp.Rotation;
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