using ReeSabers.Storages;

namespace ReeCamera {
    public readonly struct BuiltInPresetStorageOption : IStorageOption<IScenePreset> {
        #region Constructor

        public readonly IScenePreset Preset;

        public BuiltInPresetStorageOption(string optionId, string displayName, IScenePreset preset) {
            Id = optionId;
            DisplayName = displayName;
            Preset = preset;
        }

        #endregion

        #region ISelectablePresetOption Impl

        public string Id { get; }

        public string DisplayName { get; }

        public bool TryLoad(out IScenePreset result, out string failReason) {
            failReason = "";
            result = Preset;
            return true;
        }

        #endregion
    }
}