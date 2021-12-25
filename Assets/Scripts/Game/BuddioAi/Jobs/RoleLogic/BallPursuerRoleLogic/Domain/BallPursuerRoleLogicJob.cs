using Game.Buddio.Domain;
using Game.BuddioAi.Domain.EntityData;
using Game.BuddioAi.Jobs.TeamRoleAssignment.Domain;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Game.BuddioAi.Jobs.RoleLogic.BallPursuerRoleLogic.Domain {
  public struct BallPursuerRoleLogicJob : IJobParallelFor {
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

    public NativeArray<BallPursuerRoleLogicJobResult> Result;

    public void Execute(int index) {
      if (index >= this.BuddioNum) {
        return;
      }

      var role = this.TeamRoleAssignmentJobResults[index].Role;
      if (role != GameBuddioRole.BallPursuer) {
        return;
      }

      var ball = this.BallDatas[0];
      var buddio = this.BuddioDatas[index];

      var moveDir = Vector3.zero;
      if (!ball.Caught) {
        moveDir = ball.Pos - buddio.Pos;
        moveDir.y = 0;
      }

      this.Result[index] = new BallPursuerRoleLogicJobResult(moveDir.normalized);
    }
  }
}