using UnityEngine;

namespace Game.BuddioAi.Jobs.RoleLogic.ScorerRoleLogic.Domain {
  public struct ScorerRoleLogicJobDebugResult {
    public readonly Vector3 PivotPos;
    public readonly Vector4 ThrowDir;

    public ScorerRoleLogicJobDebugResult(Vector3 pivotPos, Vector4 throwDir) {
      this.PivotPos = pivotPos;
      this.ThrowDir = throwDir;
    }
  }
}