using Game.Buddio.Domain;
using Game.BuddioAi.Domain.EntityData;
using Game.BuddioAi.Jobs.TeamRoleAssignment.Domain;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Game.BuddioAi.Jobs.RoleLogic.DefenderRoleLogic.Domain {
  public struct DefenderRoleLogicJob : IJobParallelFor {
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

    public NativeArray<DefenderRoleLogicJobResult> Result;

    public void Execute(int index) {
      if (index >= this.BuddioNum) {
        return;
      }

      var role = this.TeamRoleAssignmentJobResults[index].Role;
      if (role != GameBuddioRole.Defender) {
        return;
      }
      var averagePos = Vector3.zero;

      var buddio = this.BuddioDatas[index];

      int count = 0;
      for (int i = 0; i < this.BuddioNum; ++i) {
        var otherBuddio = this.BuddioDatas[i];
        if (buddio.Id != otherBuddio.Id && buddio.Side == otherBuddio.Side) {
          averagePos += otherBuddio.Pos;
          count += 1;
        }
      }

      averagePos /= (float)count;

      var oppVec = buddio.Pos - averagePos;

      this.Result[index] = new DefenderRoleLogicJobResult(oppVec.normalized);
    }
  }
}