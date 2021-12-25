﻿using UnityEngine;

namespace Game.BuddioAi.Jobs.RoleLogic.DefenderRoleLogic.Domain {
  public struct DefenderRoleLogicJobResult {
    public readonly Vector3 MoveDir;

    public DefenderRoleLogicJobResult(Vector3 moveDir) {
      this.MoveDir = moveDir;
    }
  }
}