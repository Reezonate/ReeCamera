using JetBrains.Annotations;
using Zenject;

namespace ReeCamera {
    [UsedImplicitly]
    public class OnAppInstaller : Installer<OnAppInstaller> {
        public override void InstallBindings() {
            Container.Bind<PluginStateManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.Bind<OutputController>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.Bind<InteropCamerasManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        }
    }
}