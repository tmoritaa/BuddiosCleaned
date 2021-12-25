using Game.Applications;
using Game.Ball.Domain;
using Game.Domain;
using UnityEngine;
using Zenject;

namespace Game.Ball.Applications {
  public class BallFacadeFactory : PlaceholderFactory<int, Vector3, IBallFacade> {
    private readonly GameContext gameContext;
    private int id = GameEntityIds.BALL_START_ID;

    public BallFacadeFactory(GameContext gameContext) {
      this.gameContext = gameContext;
    }

    public IBallFacade Create(Vector3 initialPos) {
      if (this.gameContext.Balls.Entities.Count >= this.gameContext.MaxAllowedBalls) {
        Debug.LogError("BallFacade creation requested when already at max limit");
      }

      var facade = base.Create(this.id, initialPos);
      this.gameContext.AddBall(facade);

      this.id += 1;

      return facade;
    }
  }
}