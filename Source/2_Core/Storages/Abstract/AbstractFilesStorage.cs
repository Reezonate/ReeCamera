using System.IO;
using ReeSabers.Storages;

namespace ReeCamera {
    public abstract class AbstractFilesStorage<TV, TI> : AbstractStorage<TV, TI>, IMutableStorage where TI : AbstractFilesStorage<TV, TI> {
        #region FileOption

        private readonly struct FileOption : IStorageOption<TV> {
            public string AbsolutePath { get; }
            public string DisplayName { get; }
            public string Id { get; }

            public FileOption(string absolutePath) {
                AbsolutePath = absolutePath;
                DisplayName = Path.GetFileNameWithoutExtension(absolutePath);
                Id = Path.GetFileName(absolutePath);
            }

            public bool TryLoad(out TV result, out string failReason) {
                return Instance.TryLoadItem(AbsolutePath, out result, out failReason);
            }

            public bool TryDelete(out string failReason) {
                return Instance.TryRemoveItem(AbsolutePath, out failReason);
            }
        }

        #endregion

        #region Abstract

        protected abstract string DirectoryPath { get; }
        protected abstract bool TryLoadItem(string absolutePath, out TV result, out string failReason);
        protected abstract bool TryRemoveItem(string absolutePath, out string failReason);

        #endregion

        #region IMutableStorage

        public bool CanRemoveOption(ISelectableOption option) {
            return option is FileOption;
        }

        public bool TryRemoveOption(ISelectableOption option, out string failReason) {
            if (!(option is FileOption fileOption)) {
                failReason = "immutable option";
                return false;
            }

            return fileOption.TryDelete(out failReason);
        }

        #endregion

        #region FillOptionsList

        public void Refresh() {
            MarkOptionsDirty();
            UpdateOptionsIfDirty();
        }

        protected override void FillOptionsList() {
            if (!Directory.Exists(DirectoryPath)) Directory.CreateDirectory(DirectoryPath);

            foreach (var absoluteFilePath in Directory.GetFiles(DirectoryPath)) {
                OptionsList.Add(new FileOption(absoluteFilePath));
            }
        }

        #endregion
    }
}