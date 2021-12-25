namespace Game.BuddioAi.Jobs.RoleLogic.PasserRoleLogic.Domain {
  public struct PasserRoleLogicJobResult {
    public readonly int TargetBuddioId;

    public PasserRoleLogicJobResult(int targetBuddioId) {
      this.TargetBuddioId = targetBuddioId;
    }
  }
}