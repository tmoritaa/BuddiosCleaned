using Game.Buddio.Domain;
using Game.BuddioAi.Jobs.RoleLogic;

namespace Game.Buddio.Applications.RoleLogicHandling {
  public interface IGameBuddioRoleHandler {
    GameBuddioRole Role { get; }

    void Handle(RoleLogicResolutionJobResult jobResult);
  }
}