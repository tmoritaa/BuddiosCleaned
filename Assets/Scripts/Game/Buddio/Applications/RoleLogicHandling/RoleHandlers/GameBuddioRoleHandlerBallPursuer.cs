using Game.Buddio.Domain;
using Game.BuddioAi.Jobs.RoleLogic;

namespace Game.Buddio.Applications.RoleLogicHandling.RoleHandlers {
  public class GameBuddioRoleHandlerBallPursuer : IGameBuddioRoleHandler {
    private readonly GameBuddioFacade buddioFacade; // TODO: this is awful and is a circular dependency. Fine for now, but should clean up

    public GameBuddioRoleHandlerBallPursuer(GameBuddioFacade buddioFacade) {
      this.buddioFacade = buddioFacade;
    }

    public GameBuddioRole Role => GameBuddioRole.BallPursuer;

    public void Handle(RoleLogicResolutionJobResult jobResult) {
      var result = jobResult.BallPursuerRoleLogicJobResult;

      this.buddioFacade.Move(result.MoveDir.normalized);
    }
  }
}