using Game.Buddio.Domain;
using Game.BuddioAi.Domain;
using Game.BuddioAi.Jobs.RoleLogic;
using Game.Domain;

namespace Game.Buddio.Applications.RoleLogicHandling.RoleHandlers {
  public class GameBuddioRoleHandlerDisruptor : IGameBuddioRoleHandler {
    private readonly GameBuddioFacade buddio;
    private readonly IGameContextHolder gameContextHolder;

    public GameBuddioRoleHandlerDisruptor(GameBuddioFacade buddio, IGameContextHolder gameContextHolder) {
      this.buddio = buddio;
      this.gameContextHolder = gameContextHolder;
    }

    public GameBuddioRole Role => GameBuddioRole.Disruptor;

    public void Handle(RoleLogicResolutionJobResult jobResult) {
      var result = jobResult.DisruptorRoleLogicJobResult;

      var otherBuddio = this.gameContextHolder.Buddios.GetEntityWithId(result.TargetBuddioId);

      if (JobLogicHelper.IsBuddioInAttackRange(this.buddio.Buddio.Pos, this.buddio.Buddio.FacingDir, this.buddio.Buddio.Stats, otherBuddio.Buddio.Pos)) {
        this.buddio.Attack(otherBuddio.Buddio.Id);
      } else {
        var moveDir = (otherBuddio.Buddio.Pos - this.buddio.Buddio.Pos).normalized;
        this.buddio.Move(moveDir);
      }
    }
  }
}