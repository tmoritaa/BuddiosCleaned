using Game.BuddioAi.Jobs.RoleLogic.BallPursuerRoleLogic.Domain;
using Game.BuddioAi.Jobs.RoleLogic.DefenderRoleLogic.Domain;
using Game.BuddioAi.Jobs.RoleLogic.DisruptorRoleLogic.Domain;
using Game.BuddioAi.Jobs.RoleLogic.PasserRoleLogic.Domain;
using Game.BuddioAi.Jobs.RoleLogic.ScorerRoleLogic.Domain;

namespace Game.BuddioAi.Jobs.RoleLogic {
  public struct RoleLogicResolutionJobResult {
    public readonly BallPursuerRoleLogicJobResult BallPursuerRoleLogicJobResult;
    public readonly DefenderRoleLogicJobResult DefenderRoleLogicJobResult;
    public readonly DisruptorRoleLogicJobResult DisruptorRoleLogicJobResult;
    public readonly PasserRoleLogicJobResult PasserRoleLogicJobResult;
    public readonly ScorerRoleLogicJobResult ScorerRoleLogicJobResult;

    public RoleLogicResolutionJobResult(BallPursuerRoleLogicJobResult ballPursuerRoleLogicJobResult, DefenderRoleLogicJobResult defenderRoleLogicJobResult, DisruptorRoleLogicJobResult disruptorRoleLogicJobResult, PasserRoleLogicJobResult passerRoleLogicJobResult, ScorerRoleLogicJobResult scorerRoleLogicJobResult) {
      this.BallPursuerRoleLogicJobResult = ballPursuerRoleLogicJobResult;
      this.DefenderRoleLogicJobResult = defenderRoleLogicJobResult;
      this.DisruptorRoleLogicJobResult = disruptorRoleLogicJobResult;
      this.PasserRoleLogicJobResult = passerRoleLogicJobResult;
      this.ScorerRoleLogicJobResult = scorerRoleLogicJobResult;
    }
  }
}