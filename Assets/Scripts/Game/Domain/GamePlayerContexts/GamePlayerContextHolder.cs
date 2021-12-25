using System.Linq;

namespace Game.Domain.GamePlayerContexts {
  public class GamePlayerContextHolder {
    private readonly GamePlayerContext playerGamePlayerContext;
    private readonly GamePlayerContext enemyGamePlayerContext;

    public GamePlayerContextHolder(GamePlayerContext[] playerContexts) {
      this.playerGamePlayerContext = playerContexts.First(p => p.Side == Side.Player);
      this.enemyGamePlayerContext = playerContexts.First(p => p.Side == Side.Enemy);
    }

    public IGamePlayerContext GetGamePlayerContext(Side side) {
      return side == Side.Player ? (IGamePlayerContext)this.playerGamePlayerContext : this.enemyGamePlayerContext;
    }
  }
}