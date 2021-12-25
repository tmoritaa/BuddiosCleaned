using Game.Buddio.Domain;
using Game.BuddioAi.Jobs.RoleLogic;

namespace Game.Buddio.Applications.RoleLogicHandling.RoleHandlers {
  public class GameBuddioRoleHandlerDefender : IGameBuddioRoleHandler {
    private readonly GameBuddioFacade buddioFacade; // TODO: this is awful and is a circular dependency. Fine for now, but should clean up

    public GameBuddioRoleHandlerDefender(GameBuddioFacade buddioFacade) {
      this.buddioFacade = buddioFacade;
    }

    public GameBuddioRole Role => GameBuddioRole.Defender;

    public void Handle(RoleLogicResolutionJobResult jobResult) {
      var result = jobResult.DefenderRoleLogicJobResult;

      this.buddioFacade.Move(result.MoveDir.normalized);
    }
  }
}