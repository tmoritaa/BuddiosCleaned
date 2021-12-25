using BuddioCore.Domain;
using Core.States.Domain;
using Game.Ball.Applications;
using Game.Buddio.Applications.Factories;
using Game.Domain;
using Game.Domain.GamePlayerContexts;
using UnityEngine;

namespace Game.Applications.States {
  public class GameSetupState : BaseGameState {
    private readonly PlayerBuddioFacadeFactory playerBuddioFactory;
    private readonly EnemyBuddioFacadeFactory enemyBuddioFactory;
    private readonly BallFacadeFactory ballFactory;
    private readonly IStartPositionHolder startPositionHolder;
    private readonly GamePlayerContextHolder gamePlayerContextHolder;

    public GameSetupState(PlayerBuddioFacadeFactory playerBuddioFactory, EnemyBuddioFacadeFactory enemyBuddioFactory, BallFacadeFactory ballFactory, IStartPositionHolder startPositionHolder, GamePlayerContextHolder gamePlayerContextHolder) {
      this.playerBuddioFactory = playerBuddioFactory;
      this.enemyBuddioFactory = enemyBuddioFactory;
      this.ballFactory = ballFactory;
      this.startPositionHolder = startPositionHolder;
      this.gamePlayerContextHolder = gamePlayerContextHolder;
    }

    protected override void onEnter() {
      foreach (var pos in this.startPositionHolder.PlayerBuddioStartPos) {
        this.playerBuddioFactory.Create(new BuddioStats(3f, 25, 2.25f, 30), pos); // TODO: move over to scriptable object
      }

      foreach (var pos in this.startPositionHolder.EnemyBuddioStartPos) {
        this.enemyBuddioFactory.Create(new BuddioStats(2f, 25, 2.25f, 30), pos); // TODO: move over to scriptable object
      }

      this.ballFactory.Create(this.startPositionHolder.BallStartPos);

      this.gamePlayerContextHolder.GetGamePlayerContext(Side.Player).ResetScore();
      this.gamePlayerContextHolder.GetGamePlayerContext(Side.Enemy).ResetScore();
    }

    protected override void onExit() {
    }
  }
}