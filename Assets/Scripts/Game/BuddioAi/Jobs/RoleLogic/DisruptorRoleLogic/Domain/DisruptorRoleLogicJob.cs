using Game.Buddio.Domain;
using Game.BuddioAi.Domain;
using Game.BuddioAi.Domain.EntityData;
using Game.BuddioAi.Jobs.TeamRoleAssignment.Domain;
using Unity.Collections;
using Unity.Jobs;

namespace Game.BuddioAi.Jobs.RoleLogic.DisruptorRoleLogic.Domain {
  public struct DisruptorRoleLogicJob : IJobParallelFor {
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

    public NativeArray<DisruptorRoleLogicJobResult> Result;

    public void Execute(int index) {
      if (index >= this.BuddioNum) {
        return;
      }

      var role = this.TeamRoleAssignmentJobResults[index].Role;
      if (role != GameBuddioRole.Disruptor) {
        return;
      }

      var buddio = this.BuddioDatas[index];

      int targetBuddioId = -1;
      for (int i = 0; i < this.BuddioNum; ++i) {
        var otherBuddio = this.BuddioDatas[i];

        if (buddio.Side == otherBuddio.Side) {
          continue;
        }

        if (JobLogicHelper.IsBuddioInAttackRange(buddio.Pos, buddio.FacingDir, buddio.BuddioStats, otherBuddio.Pos)) {
          targetBuddioId = otherBuddio.Id;
          break;
        } else {
          if (targetBuddioId < 0) {
            targetBuddioId = otherBuddio.Id;
          } else {
            var dist = (buddio.Pos - otherBuddio.Pos).sqrMagnitude;

            var curClosestBuddio = this.BuddioDatas[targetBuddioId];
            var curDist = (buddio.Pos - curClosestBuddio.Pos).sqrMagnitude;

            if (curDist > dist) {
              targetBuddioId = otherBuddio.Id;
            }
          }
        }
      }

      this.Result[index] = new DisruptorRoleLogicJobResult(targetBuddioId);
    }
  }
}