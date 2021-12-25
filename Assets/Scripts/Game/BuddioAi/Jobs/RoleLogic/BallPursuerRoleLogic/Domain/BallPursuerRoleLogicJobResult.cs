using UnityEngine;

namespace Game.BuddioAi.Jobs.RoleLogic.BallPursuerRoleLogic.Domain {
  public struct BallPursuerRoleLogicJobResult {
    public readonly Vector3 MoveDir;

    public BallPursuerRoleLogicJobResult(Vector3 moveDir) {
      this.MoveDir = moveDir;
    }
  }
}