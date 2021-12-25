using Game.Ball.Domain;
using Game.Buddio.Domain;
using Game.Domain.GamePlayerContexts;

namespace Game.Domain {
  public interface IGameContextHolder {
    IEntityHolder<IGameBuddioFacade> Buddios { get; }
    IEntityHolder<IBallFacade> Balls { get; }

    int MaxAllowedBuddios { get; }
    int MaxAllowedBalls { get; }
  }
}