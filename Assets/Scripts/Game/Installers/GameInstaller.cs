using BuddioCore.Domain;
using Core.States.Domain;
using Core.States.Installers;
using Game.Applications.Events;
using Game.Applications.States;
using Game.Ball.Applications;
using Game.Ball.Domain;
using Game.Ball.Installers;
using Game.Buddio.Applications.Factories;
using Game.Buddio.Domain;
using Game.Buddio.Installers;
using Game.BuddioAi.Installers;
using Game.Domain;
using Game.Domain.GamePlayerContexts;
using Game.GameDebug;
using Game.Infrastructure;
using UnityEngine;
using Zenject;

namespace Game.Installers {
  public class GameInstaller : MonoInstaller {
    [SerializeField] private GameObject playerBuddioPrefab = null;
    [SerializeField] private GameObject enemyBuddioPrefab = null;
    [SerializeField] private Transform buddiosRoot = null;

    [SerializeField] private GameObject ballPrefab = null;
    [SerializeField] private Transform ballRoot = null;

    public override void InstallBindings() {
      this.Container.DeclareSignal<GameplayStartEvent>();
      this.Container.DeclareSignal<GameplayStopEvent>();
      this.Container.DeclareSignal<ResetForNewRoundEvent>();
      this.Container.DeclareSignal<TeamScoredEvent>().OptionalSubscriber();

      GameStateInstaller.Install(this.Container);
      GameBuddioAiInstaller.Install(this.Container);

      this.Container.BindInstances(new GamePlayerContext(Side.Player), new GamePlayerContext(Side.Enemy));
      this.Container.BindInterfacesAndSelfTo<GamePlayerContextHolder>().AsSingle();

      this.Container.BindInterfacesTo<TempStartPositionHolder>().AsSingle();

      this.Container.BindInterfacesAndSelfTo<GameContext>().AsSingle();

      this.Container.BindInterfacesAndSelfTo<GameSetupState>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<GamePlayState>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<GameResetForNewRoundState>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle();

      this.Container.BindInterfacesTo<GameStarter<GameStateMachine>>().AsSingle();

      this.Container.BindFactory<int, Side, BuddioStats, Vector3, IGameBuddioFacade, PlayerBuddioFacadeFactory>()
        .FromSubContainerResolve()
        .ByNewPrefabInstaller<GameBuddioInstaller>(this.playerBuddioPrefab)
        .UnderTransform(this.buddiosRoot)
        .AsSingle();

      this.Container.BindFactory<int, Side, BuddioStats, Vector3, IGameBuddioFacade, EnemyBuddioFacadeFactory>()
        .FromSubContainerResolve()
        .ByNewPrefabInstaller<GameBuddioInstaller>(this.enemyBuddioPrefab)
        .UnderTransform(this.buddiosRoot)
        .AsSingle();

      this.Container.BindFactory<int, Vector3, IBallFacade, BallFacadeFactory>()
        .FromSubContainerResolve()
        .ByNewPrefabInstaller<BallInstaller>(this.ballPrefab)
        .UnderTransform(this.ballRoot)
        .AsSingle();
    }
  }
}