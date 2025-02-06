using System.Collections.Generic;

namespace ReeCamera {
    public interface IScenePreset {
        MainCameraConfig MainCamera { get; }
        IReadOnlyList<SecondaryCameraConfig> SecondaryCameras { get; }
    }
}