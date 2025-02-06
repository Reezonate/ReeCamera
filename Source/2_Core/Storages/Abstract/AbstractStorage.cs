using System;
using System.Collections.Generic;
using ReeSabers.Storages;

namespace ReeCamera {
    public abstract class AbstractStorage<TV, TI> : IStorage where TI : AbstractStorage<TV, TI> {
        #region Instance

        private static TI _instance;

        public static TI Instance {
            get {
                if (_instance != null) return _instance;
                _instance = Activator.CreateInstance<TI>();
                return _instance;
            }
        }

        #endregion

        #region Options

        private bool _optionsDirty = true;
        protected readonly List<IStorageOption<TV>> OptionsList = new List<IStorageOption<TV>>();

        protected abstract void FillOptionsList();

        protected void MarkOptionsDirty() {
            _optionsDirty = true;
        }

        protected void UpdateOptionsIfDirty() {
            if (!_optionsDirty) return;
            OptionsList.Clear();
            FillOptionsList();
            _optionsDirty = false;
        }

        public bool TryGetOption(string optionId, out IStorageOption<TV> result) {
            foreach (var option in GetAllOptions()) {
                if (option.Id != optionId) continue;
                result = option;
                return true;
            }

            result = default;
            return false;
        }

        public IStorageOption<TV> GetOptionOrDefault(string optionId, IStorageOption<TV> defaultOption = default) {
            foreach (var option in GetAllOptions()) {
                if (option.Id != optionId) continue;
                return option;
            }

            return defaultOption;
        }

        public IReadOnlyList<ISelectableOption> GetAllOptionsAbstract(bool refresh) {
            if (refresh) MarkOptionsDirty();
            return GetAllOptions();
        }

        public IReadOnlyList<IStorageOption<TV>> GetAllOptions() {
            UpdateOptionsIfDirty();
            return OptionsList;
        }

        #endregion
    }
}