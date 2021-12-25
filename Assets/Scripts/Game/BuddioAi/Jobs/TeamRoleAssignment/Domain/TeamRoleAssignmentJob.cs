using Game.Buddio.Domain;
using Game.BuddioAi.Domain.EntityData;
using Unity.Collections;
using Unity.Jobs;

namespace Game.BuddioAi.Jobs.TeamRoleAssignment.Domain {
  public struct TeamRoleAssignmentJob : IJobParallelFor {
    [ReadOnly]
    public NativeArray<GameBuddioData> BuddioDatas;

    [ReadOnly]
    public NativeArray<GameBallData> BallDatas;

    [ReadOnly]
    public int BuddioNum;

    [ReadOnly]
    public int BallNum;

    public NativeArray<TeamRoleAssignmentJobResult> Results;

    public void Execute(int index) {
      if (index >= this.BuddioNum) {
        return;
      }

      var buddioData = this.BuddioDatas[index];
      if (buddioData.HasBall) {
        this.Results[index] = new TeamRoleAssignmentJobResult(GameBuddioRole.Scorer);
        return;
      }

      var ballIsCaught = this.BallDatas[0].Caught;
      this.Results[index] = new TeamRoleAssignmentJobResult(ballIsCaught ? GameBuddioRole.Defender : GameBuddioRole.BallPursuer);
    }
  }
}