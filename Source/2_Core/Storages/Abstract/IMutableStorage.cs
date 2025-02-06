namespace ReeSabers.Storages {
    public interface IMutableStorage {
        bool CanRemoveOption(ISelectableOption option);
        bool TryRemoveOption(ISelectableOption option, out string failReason);
    }
}