using ReeSabers.Storages;
using ReeSabers.UI;

namespace ReeCamera.UI {
    public class StorageField : AbstractButtonField<ISelectableOption, StorageInputContext> {
        protected override void OnEditButtonWasClicked() {
            ReeModalSystem.OpenModal<StorageBrowser>(RootNode, Context);
        }

        protected override string FormatValue(ISelectableOption value) {
            return value?.DisplayName ?? "none";
        }
    }
}