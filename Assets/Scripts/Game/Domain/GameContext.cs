using System.Collections.Generic;
using System.Linq;
using Game.Ball.Applications;
using Game.Ball.Domain;
using Game.Buddio.Applications;
using Game.Buddio.Domain;
using Game.Domain.GamePlayerContexts;
using UnityEngine;

namespace Game.Domain {
  public class GameContext : IGameContextHolder {
    private readonly EntityContainer<IGameBuddioFacade> buddios = new EntityContainer<IGameBuddioFacade>();
    private readonly EntityContainer<IBallFacade> balls = new EntityContainer<IBallFacade>();

    public IEntityHolder<IGameBuddioFacade> Buddios => this.buddios;
    public IEntityHolder<IBallFacade> Balls => this.balls;

    public int MaxAllowedBuddios => 8;
    public int MaxAllowedBalls => 1;

    public void AddBuddio(IGameBuddioFacade buddio) {
      this.buddios.AddEntity(buddio);
    }

    public void AddBall(IBallFacade ball) {
      this.balls.AddEntity(ball);
    }
  }
}