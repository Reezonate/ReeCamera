using BeatSaberMarkupLanguage.GameplaySetup;
using ReeSabers.UI;
using Zenject;

namespace ReeCamera.UI {
    public class ModPanelUIHelper : IInitializable {
        public void Initialize() {
            AddTab();
        }

        #region Tab management

        private static string ResourcePath => $"{Plugin.ResourcesPath}.BSML.ModPanel.ModPanelUI.bsml";
        public static string TabName => Plugin.FancyName;

        private bool _tabActive;

        public void AddTab() {
            if (_tabActive) return;
            GameplaySetup.instance.AddTab(
                TabName,
                ResourcePath,
                PepegaSingletonFix<ModPanelUI>.instance
            );
            _tabActive = true;
        }

        public void RemoveTab() {
            if (!_tabActive) return;
            GameplaySetup.instance.RemoveTab(TabName);
            _tabActive = false;
        }

        #endregion
    }
}