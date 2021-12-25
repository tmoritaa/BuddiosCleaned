using System;
using System.Runtime.CompilerServices;
using Game.BuddioAi.Domain.EntityData;
using Game.Domain;
using Unity.Collections;
using Unity.Jobs;

namespace Game.BuddioAi.Jobs.TeamRoleAssignment.Domain {
  public class TeamRoleAssignmentJobScheduler : IDisposable {
    private readonly IGameContextHolder gameContextHolder;

    private TeamRoleAssignmentJob job;

    public NativeArray<TeamRoleAssignmentJobResult> Results;

    public TeamRoleAssignmentJobScheduler(IGameContextHolder gameContextHolder) {
      this.gameContextHolder = gameContextHolder;

      this.Results = new NativeArray<TeamRoleAssignmentJobResult>(this.gameContextHolder.MaxAllowedBuddios, Allocator.Persistent);

      this.job.Results = this.Results;
    }

    public void Setup(int buddioNum, NativeArray<GameBuddioData> buddios, int ballNum, NativeArray<GameBallData> balls) {
      this.job.BuddioNum = buddioNum;
      this.job.BuddioDatas = buddios;

      this.job.BallNum = ballNum;
      this.job.BallDatas = balls;
    }

    public JobHandle ScheduleJob() {
      var jobHandle = this.job.Schedule(this.job.BuddioNum, 1);

      return jobHandle;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TeamRoleAssignmentJobResult GetJobResult(int idx) {
      return this.Results[idx];
    }

    public void Dispose() {
      this.Results.Dispose();
    }
  }
}