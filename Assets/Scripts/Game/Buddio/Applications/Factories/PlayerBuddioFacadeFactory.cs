using Game.Applications;
using Game.Domain;

namespace Game.Buddio.Applications.Factories {
  public class PlayerBuddioFacadeFactory : BaseBuddioFacadeFactory {
    public PlayerBuddioFacadeFactory(GameContext gameContext) : base(Side.Player, gameContext, GameEntityIds.PLAYER_BUDDIO_START_ID) {
    }
  }
}