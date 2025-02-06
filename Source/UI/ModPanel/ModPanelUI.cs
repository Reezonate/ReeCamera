using BeatSaberMarkupLanguage.Attributes;
using JetBrains.Annotations;
using ReeCamera;
using ReeCamera.UI;
using ReeSabers.Storages;
using UnityEngine;

namespace ReeSabers.UI {
    internal class ModPanelUI : PepegaSingletonFix<ModPanelUI> {
        #region Components

        [UIObject("Root"), UsedImplicitly]
        private GameObject _root;

        [UIValue("MainMenuVRPresetField"), UsedImplicitly]
        private StorageField _mainMenuVRPresetField;

        [UIValue("MainMenuFPFCPresetField"), UsedImplicitly]
        private StorageField _mainMenuFPFCPresetField;

        [UIValue("GameplayVRPresetField"), UsedImplicitly]
        private StorageField _gameplayVRPresetField;

        [UIValue("GameplayFPFCPresetField"), UsedImplicitly]
        private StorageField _gameplayFPFCPresetField;

        private void Awake() {
            _mainMenuVRPresetField = ReeUIComponentV2.Instantiate<StorageField>(transform);
            _mainMenuFPFCPresetField = ReeUIComponentV2.Instantiate<StorageField>(transform);
            _gameplayVRPresetField = ReeUIComponentV2.Instantiate<StorageField>(transform);
            _gameplayFPFCPresetField = ReeUIComponentV2.Instantiate<StorageField>(transform);
        }

        #endregion

        #region PostParse

        private bool _parsed;

        [UIAction("#post-parse"), UsedImplicitly]
        private void PostParse() {
            _parsed = true;
            if (_root.GetComponent<AutoSaver>() == null) _root.AddComponent<AutoSaver>();

            SetPreset(_mainMenuVRFieldContext, MainPluginConfig.Instance.MainMenuConfigVR);
            SetPreset(_mainMenuFPFCFieldContext, MainPluginConfig.Instance.MainMenuConfigFPFC);
            SetPreset(_gameplayVRFieldContext, MainPluginConfig.Instance.GameplayConfigVR);
            SetPreset(_gameplayFPFCFieldContext, MainPluginConfig.Instance.GameplayConfigFPFC);

            _mainMenuVRPresetField.SetContext(_mainMenuVRFieldContext);
            _mainMenuFPFCPresetField.SetContext(_mainMenuFPFCFieldContext);
            _gameplayVRPresetField.SetContext(_gameplayVRFieldContext);
            _gameplayFPFCPresetField.SetContext(_gameplayFPFCFieldContext);

            _mainMenuVRFieldContext.AddStateListener(OnMainMenuVRPresetChanged, this);
            _mainMenuFPFCFieldContext.AddStateListener(OnMainMenuFPFCPresetChanged, this);
            _gameplayVRFieldContext.AddStateListener(OnGameplayVRPresetChanged, this);
            _gameplayFPFCFieldContext.AddStateListener(OnGameplayFPFCPresetChanged, this);

            return;

            void SetPreset(StorageInputContext context, AbstractSceneConfig sceneConfig) {
                var selectedOption = PresetsStorage.Instance.GetOptionOrDefault(sceneConfig.PresetIdOV.Value);

                if (selectedOption == null) {
                    sceneConfig.PresetIdOV.SetValue("", sceneConfig.PresetIdOV.LastChangeSource);
                    selectedOption = PresetsStorage.EmptyPresetOption;
                }

                context.SetValue(selectedOption, this);
            }
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            if (!_parsed) return;

            _mainMenuVRFieldContext.RemoveStateListener(OnMainMenuVRPresetChanged);
            _mainMenuFPFCFieldContext.RemoveStateListener(OnMainMenuFPFCPresetChanged);
            _gameplayVRFieldContext.RemoveStateListener(OnGameplayVRPresetChanged);
            _gameplayFPFCFieldContext.RemoveStateListener(OnGameplayFPFCPresetChanged);
        }

        internal class AutoSaver : MonoBehaviour {
            private void OnDisable() {
                MainPluginConfig.Save(Plugin.MainConfigPath);
            }
        }

        #endregion

        #region Contexts

        private readonly StorageInputContext _mainMenuVRFieldContext = new StorageInputContext() {
            Header = "Main Menu VR Preset",
            Label = "Main Menu",
            Storage = PresetsStorage.Instance
        };

        private readonly StorageInputContext _mainMenuFPFCFieldContext = new StorageInputContext() {
            Header = "Main Menu FPFC Preset",
            Label = "Main Menu",
            Storage = PresetsStorage.Instance
        };

        private readonly StorageInputContext _gameplayVRFieldContext = new StorageInputContext() {
            Header = "Gameplay VR Preset",
            Label = "Gameplay",
            Storage = PresetsStorage.Instance
        };

        private readonly StorageInputContext _gameplayFPFCFieldContext = new StorageInputContext() {
            Header = "Gameplay FPFC Preset",
            Label = "Gameplay",
            Storage = PresetsStorage.Instance
        };

        #endregion

        #region Events

        private void OnMainMenuVRPresetChanged(ISelectableOption value, ObservableValueState state) {
            MainPluginConfig.Instance.MainMenuConfigVR.PresetIdOV.SetValue(value.Id, this);
        }

        private void OnMainMenuFPFCPresetChanged(ISelectableOption value, ObservableValueState state) {
            MainPluginConfig.Instance.MainMenuConfigFPFC.PresetIdOV.SetValue(value.Id, this);
        }

        private void OnGameplayVRPresetChanged(ISelectableOption value, ObservableValueState state) {
            MainPluginConfig.Instance.GameplayConfigVR.PresetIdOV.SetValue(value.Id, this);
        }

        private void OnGameplayFPFCPresetChanged(ISelectableOption value, ObservableValueState state) {
            MainPluginConfig.Instance.GameplayConfigFPFC.PresetIdOV.SetValue(value.Id, this);
        }

        [UIAction("ReloadOnClick"), UsedImplicitly]
        private void ReloadOnClick() {
            MainPluginConfig.MassReload();
        }

        [UIAction("SaveAllOnClick"), UsedImplicitly]
        private void SaveAllOnClick() {
            MainPluginConfig.SaveAll();
        }

        #endregion
    }
}