namespace ReeSabers.Storages {
    public interface IStorageOption<T> : ISelectableOption {
        bool TryLoad(out T result, out string failReason);
    }
}