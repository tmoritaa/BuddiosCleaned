using BuddioCore.Domain;
using Game.Buddio.Domain;
using Game.Domain;
using UnityEngine;
using Zenject;

namespace Game.Buddio.Applications.Factories {
  public abstract class BaseBuddioFacadeFactory : PlaceholderFactory<int, Side, BuddioStats, Vector3, IGameBuddioFacade> {
    private readonly GameContext gameContext;
    private readonly Side side;
    private int curId;

    protected BaseBuddioFacadeFactory(Side side, GameContext gameContext, int startId) {
      this.gameContext = gameContext;
      this.side = side;
      this.curId = startId;
    }

    public IGameBuddioFacade Create(BuddioStats buddioStats, Vector3 initialPos) {
      if (this.gameContext.Buddios.Entities.Count >= this.gameContext.MaxAllowedBuddios) {
        Debug.LogError("GameBuddioFacade creation requested when already at max limit");
      }

      var facade = base.Create(this.curId, this.side, buddioStats, initialPos);
      this.gameContext.AddBuddio(facade);

      this.curId += 1;

      return facade;
    }
  }
}