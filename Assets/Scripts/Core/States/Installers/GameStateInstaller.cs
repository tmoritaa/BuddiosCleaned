using Core.States.Infrastructure;
using Zenject;

namespace Core.States.Installers {
  public class GameStateInstaller : Installer<GameStateInstaller> {
    public override void InstallBindings() {
      this.Container.BindInterfacesTo<UniTaskStateMachineTransitionPoller>().AsTransient();
    }
  }
}