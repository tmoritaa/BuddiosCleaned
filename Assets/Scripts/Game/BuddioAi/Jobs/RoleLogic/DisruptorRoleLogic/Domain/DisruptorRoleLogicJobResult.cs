using UnityEngine;

namespace Game.BuddioAi.Jobs.RoleLogic.DisruptorRoleLogic.Domain {
  public struct DisruptorRoleLogicJobResult {
    public readonly int TargetBuddioId;

    public DisruptorRoleLogicJobResult(int targetBuddioId) {
      this.TargetBuddioId = targetBuddioId;
    }
  }
}