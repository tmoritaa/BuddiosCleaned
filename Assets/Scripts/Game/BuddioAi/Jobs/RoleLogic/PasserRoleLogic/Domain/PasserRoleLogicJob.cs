using Game.Buddio.Domain;
using Game.BuddioAi.Domain.EntityData;
using Game.BuddioAi.Jobs.TeamRoleAssignment.Domain;
using Unity.Collections;
using Unity.Jobs;

namespace Game.BuddioAi.Jobs.RoleLogic.PasserRoleLogic.Domain {
  public struct PasserRoleLogicJob : IJobParallelFor {
    [ReadOnly]
    public NativeArray<GameBuddioData> BuddioDatas;

    [ReadOnly]
    public int BuddioNum;

    [ReadOnly]
    public NativeArray<GameBallData> BallDatas;

    [ReadOnly]
    public int BallNum;

    [ReadOnly]
    public NativeArray<TeamRoleAssignmentJobResult> TeamRoleAssignmentJobResults;

    public NativeArray<PasserRoleLogicJobResult> Result;

    public void Execute(int index) {
      if (index >= this.BuddioNum) {
        return;
      }

      var role = this.TeamRoleAssignmentJobResults[index].Role;
      if (role != GameBuddioRole.Passer) {
        return;
      }

      var buddio = this.BuddioDatas[index];

      int passTargetIdx = -1;

      for (int i = 0; i < this.BuddioNum; ++i) {
        var otherBuddio = this.BuddioDatas[i];

        if (buddio.Side == otherBuddio.Side && buddio.Id != otherBuddio.Id) {
          passTargetIdx = otherBuddio.Id;
          break;
        }
      }

      this.Result[index] = new PasserRoleLogicJobResult(passTargetIdx);
    }
  }
}