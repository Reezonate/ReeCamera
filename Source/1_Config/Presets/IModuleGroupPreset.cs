using System.Collections.Generic;

namespace ReeCamera {
    public interface IScenePreset {
        IReadOnlyList<SceneLayoutConfig> LayoutConfigs { get; }
    }
}