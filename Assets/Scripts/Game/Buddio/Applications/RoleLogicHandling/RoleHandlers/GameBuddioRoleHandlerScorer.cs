using Game.Buddio.Domain;
using Game.BuddioAi.Jobs.RoleLogic;
using UnityEngine;

namespace Game.Buddio.Applications.RoleLogicHandling.RoleHandlers {
  public class GameBuddioRoleHandlerScorer : IGameBuddioRoleHandler {
    private readonly GameBuddio buddio;
    private readonly GameBuddioFacade buddioFacade; // TODO: this is awful and is a circular dependency. Fine for now, but should clean up

    public GameBuddioRoleHandlerScorer(GameBuddio buddio, GameBuddioFacade buddioFacade) {
      this.buddio = buddio;
      this.buddioFacade = buddioFacade;
    }

    public GameBuddioRole Role => GameBuddioRole.Scorer;

    public void Handle(RoleLogicResolutionJobResult jobResult) {
      var result = jobResult.ScorerRoleLogicJobResult;

      if (this.buddioFacade.Buddio.HasBall) {
        this.buddio.SetMoveDir(Vector2.zero);

        if (result.ScoreThrowExists) {
          this.buddioFacade.Throw(result.ThrowPos);
        }
      }
    }
  }
}