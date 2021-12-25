using Domain;
using Domain.Factories;
using Infrastructure;
using Zenject;

namespace Program {
  public class ProgramInstaller : MonoInstaller {
    public override void InstallBindings() {
      SignalBusInstaller.Install(Container);

      this.Container.BindFactoryCustomInterface<ITimer, UniTaskTimer.Factory, ITimerFactory>().To<UniTaskTimer>();
    }
  }
}