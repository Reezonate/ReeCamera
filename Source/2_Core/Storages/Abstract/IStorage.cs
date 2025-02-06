using System.Collections.Generic;

namespace ReeSabers.Storages {
    public interface IStorage {
        IReadOnlyList<ISelectableOption> GetAllOptionsAbstract(bool refresh);
    }
}