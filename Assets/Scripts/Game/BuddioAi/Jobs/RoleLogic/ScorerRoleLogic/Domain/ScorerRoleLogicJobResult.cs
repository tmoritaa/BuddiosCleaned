using UnityEngine;

namespace Game.BuddioAi.Jobs.RoleLogic.ScorerRoleLogic.Domain {
  public struct ScorerRoleLogicJobResult {
    public readonly bool ScoreThrowExists;
    public readonly Vector3 ThrowPos;

    public ScorerRoleLogicJobResult(bool scoreThrowExists, Vector3 throwPos) {
      this.ScoreThrowExists = scoreThrowExists;
      this.ThrowPos = throwPos;
    }
  }
}