using ReeSabers.Storages;

namespace ReeCamera.UI {
    public class StorageInputContext : InputContext<ISelectableOption> {
        #region Values

        public IStorage Storage;

        #endregion

        #region Constructors

        public StorageInputContext() { }
        public StorageInputContext(ISelectableOption initialValue) : base(initialValue) { }
        public StorageInputContext(IObservableValue<ISelectableOption> value) : base(value) { }

        #endregion
    }
}