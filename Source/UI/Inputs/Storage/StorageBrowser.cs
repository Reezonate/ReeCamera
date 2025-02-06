using System.Collections.Generic;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using JetBrains.Annotations;
using ReeSabers.Storages;
using TMPro;
using UnityEngine;

namespace ReeCamera.UI {
    public class StorageBrowser : AbstractInputModal<ISelectableOption, StorageInputContext> {
        #region Components

        [UIComponent("HeaderText"), UsedImplicitly]
        private TextMeshProUGUI _headerText;

        [UIComponent("OptionsList"), UsedImplicitly]
        private CustomListTableData _optionsList;

        [UIObject("TexturePreviewRoot"), UsedImplicitly]
        private GameObject _texturePreviewRoot;

        private void Update() {
            UpdateListIfDirty();
            UpdateHighlightIfDirty();
        }

        #endregion

        #region Events

        protected override void OnContextChanged() {
            _headerText.text = Context.Header;
            MarkListDirty();
            MarkHighlightDirty();
        }

        protected override void OnValueChanged(ISelectableOption value, ObservableValueState state) {
            MarkHighlightDirty();
        }

        [UIAction("OnOptionSelected"), UsedImplicitly]
        private void OnOptionSelected(TableView tableView, int index) {
            Context?.PreviewValue(_options[index], this);
            MarkHighlightDirty();
        }

        #endregion

        #region List

        private IReadOnlyList<ISelectableOption> _options = [];
        private bool _listDirty = true;

        private void MarkListDirty() {
            _listDirty = true;
        }

        private void UpdateListIfDirty() {
            if (!_listDirty) return;
            _listDirty = false;

            _options = Context.Storage.GetAllOptionsAbstract(true);

            var cellInfos = new List<CustomListTableData.CustomCellInfo>();

            foreach (var option in _options) {
                var cellInfo = new CustomListTableData.CustomCellInfo(option.DisplayName);
                cellInfos.Add(cellInfo);
            }

            _optionsList.Data = cellInfos;
            _optionsList.TableView.ReloadDataKeepingPosition();

            MarkHighlightDirty();
        }

        #endregion

        #region Highlight

        private bool _highlightDirty = true;

        private void MarkHighlightDirty() {
            _highlightDirty = true;
        }

        private void UpdateHighlightIfDirty() {
            if (!_highlightDirty) return;
            _highlightDirty = false;

            var selectedOption = Context?.Value;

            if (selectedOption == null) {
                _optionsList.TableView.ClearSelection();
            } else {
                for (var i = 0; i < _options.Count; i++) {
                    if (_options[i].Id != selectedOption.Id) continue;
                    _optionsList.TableView.SelectCellWithIdx(i);
                    break;
                }
            }
        }

        #endregion
    }
}