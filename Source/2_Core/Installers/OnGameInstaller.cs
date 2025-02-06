using JetBrains.Annotations;
using Zenject;

namespace ReeCamera {
    [UsedImplicitly]
    public class OnGameInstaller : Installer<OnGameInstaller> {
        public override void InstallBindings() {
            Container.Bind<GameSceneManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        }
    }
}