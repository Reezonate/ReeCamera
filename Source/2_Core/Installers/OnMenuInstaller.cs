using JetBrains.Annotations;
using ReeCamera.UI;
using SiraUtil.Tools.FPFC;
using Zenject;

namespace ReeCamera {
    [UsedImplicitly]
    public class OnMenuInstaller : Installer<OnMenuInstaller> {
        [Inject, UsedImplicitly]
        private IVRPlatformHelper _vrPlatformHelper;
        
        [Inject, UsedImplicitly]
        private IFPFCSettings _fpfcSettings;

        public override void InstallBindings() {
            if (_fpfcSettings.Enabled) {
                ReeSabersInterop.SuppressReeSabersFramerateManager = true;
                PluginState.LaunchTypeOV.SetValue(LaunchType.FPFC, this);
            } else {
                ReeSabersInterop.SuppressReeSabersFramerateManager = false;
                PluginState.LaunchTypeOV.SetValue(LaunchType.VR, this);
            }

            Container.Bind<CameraPrefabManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.Bind<MenuSceneManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ModPanelUIHelper>().AsSingle();
        }
    }
}