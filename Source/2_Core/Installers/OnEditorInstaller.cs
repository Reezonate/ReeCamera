using JetBrains.Annotations;
using Zenject;

namespace ReeCamera;

[UsedImplicitly]
public class OnEditorInstaller : Installer<OnEditorInstaller> {
    public override void InstallBindings() {
        Container.Bind<BeatmapEditorSceneManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }
}