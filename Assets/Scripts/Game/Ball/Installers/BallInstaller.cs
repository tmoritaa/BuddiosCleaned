using Game.Ball.Applications;
using Game.Ball.Controllers;
using Game.Ball.Domain;
using UnityEngine;
using Zenject;

namespace Game.Ball.Installers {
  public class BallInstaller : Installer<BallInstaller> {
    [Inject] private readonly int id = 0;
    [Inject] private readonly Vector3 initialPos = Vector3.zero;

    public override void InstallBindings() {
      this.Container.BindInterfacesAndSelfTo<GameBall>().AsSingle();
      this.Container.BindInstance<int>(this.id).WhenInjectedInto<GameBall>();

      this.Container.BindInterfacesTo<BallFacade>().FromComponentOnRoot();
      this.Container.BindInstance(this.initialPos).WhenInjectedInto<BallFacade>();

      this.Container.Bind<Transform>().FromComponentOnRoot();
      this.Container.Bind<Rigidbody>().FromComponentOnRoot();

      this.Container.BindInterfacesAndSelfTo<BallController>().AsSingle();
    }
  }
}