using Game.Buddio.Domain;
using Game.BuddioAi.Jobs.RoleLogic;
using Game.Domain;

namespace Game.Buddio.Applications.RoleLogicHandling.RoleHandlers {
  public class GameBuddioRoleHandlerPasser : IGameBuddioRoleHandler {
    private readonly GameBuddio buddio;
    private readonly IGameContextHolder gameContextHolder;
    private readonly GameBuddioFacade buddioFacade; // TODO: this is awful and is a circular dependency. Fine for now, but should clean up

    public GameBuddioRoleHandlerPasser(GameBuddio buddio, IGameContextHolder gameContextHolder, GameBuddioFacade buddioFacade) {
      this.buddio = buddio;
      this.gameContextHolder = gameContextHolder;
      this.buddioFacade = buddioFacade;
    }

    public GameBuddioRole Role => GameBuddioRole.Passer;

    public void Handle(RoleLogicResolutionJobResult jobResult) {
      var result = jobResult.PasserRoleLogicJobResult;

      if (this.buddioFacade.Buddio.HasBall) {
        this.buddioFacade.Pass(result.TargetBuddioId);
      }
    }
  }
}