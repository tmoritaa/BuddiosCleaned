using BuddioCore.Domain;
using Core.ActionResolvers;
using Game.Buddio.Applications;
using Game.Buddio.Applications.ActionResolvers;
using Game.Buddio.Applications.RoleLogicHandling;
using Game.Buddio.Applications.RoleLogicHandling.RoleHandlers;
using Game.Buddio.Controllers;
using Game.Buddio.Domain;
using Game.Buddio.Inputs;
using Game.Domain;
using UnityEngine;
using Zenject;

namespace Game.Buddio.Installers {
  public class GameBuddioInstaller : Installer<GameBuddioInstaller> {
    [Inject] private readonly int id = 0;
    [Inject] private readonly Side side = Side.Player;
    [Inject] private readonly BuddioStats buddioStats = new BuddioStats();
    [Inject] private readonly Vector3 initialPos = Vector3.zero;

    public override void InstallBindings() {
      this.Container.BindInterfacesAndSelfTo<GameBuddio>().AsSingle();
      this.Container.BindInstance(this.id).WhenInjectedInto<GameBuddio>();
      this.Container.BindInstance(this.side).WhenInjectedInto<GameBuddio>();

      this.Container.BindInstance(this.buddioStats);

      this.Container.BindInterfacesAndSelfTo<ActionResolverHolder>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<GameBuddioActionResolverRunner>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<ThrowActionResolver>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<CatchActionResolver>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<AttackActionResolver>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<HitStunActionResolver>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<PassActionResolver>().AsSingle();

      this.Container.BindInterfacesAndSelfTo<GameBuddioRoleHandlerDelegator>().AsSingle();
      this.Container.BindInterfacesTo<GameBuddioRoleHandlerIdle>().AsSingle();
      this.Container.BindInterfacesTo<GameBuddioRoleHandlerBallPursuer>().AsSingle();
      this.Container.BindInterfacesTo<GameBuddioRoleHandlerPasser>().AsSingle();
      this.Container.BindInterfacesTo<GameBuddioRoleHandlerScorer>().AsSingle();
      this.Container.BindInterfacesTo<GameBuddioRoleHandlerDisruptor>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<GameBuddioRoleHandlerDefender>().AsSingle();

      this.Container.BindInterfacesAndSelfTo<GameBuddioFacade>().FromComponentOnRoot();
      this.Container.BindInstance(this.initialPos).WhenInjectedInto<GameBuddioFacade>();

      this.Container.Bind<Transform>().FromComponentOnRoot();
      this.Container.Bind<Rigidbody>().FromComponentOnRoot();
      this.Container.Bind<BuddioActuator>().FromComponentOnRoot();

      this.Container.BindInterfacesAndSelfTo<BuddioEventDelegator>().FromComponentsInChildren();

      this.Container.BindInterfacesAndSelfTo<BuddioEventListener>().AsSingle();

      this.Container.BindInterfacesAndSelfTo<BuddioController>().AsSingle();
    }
  }
}
