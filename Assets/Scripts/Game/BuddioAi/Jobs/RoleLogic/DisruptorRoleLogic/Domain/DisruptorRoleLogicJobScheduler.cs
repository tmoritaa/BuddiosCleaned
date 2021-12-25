using System;
using System.Runtime.CompilerServices;
using Game.BuddioAi.Domain.EntityData;
using Game.BuddioAi.Jobs.TeamRoleAssignment.Domain;
using Game.Domain;
using Unity.Collections;
using Unity.Jobs;

namespace Game.BuddioAi.Jobs.RoleLogic.DisruptorRoleLogic.Domain {
  public class DisruptorRoleLogicJobScheduler : IDisposable {
    private readonly IGameContextHolder gameContextHolder;

    private NativeArray<DisruptorRoleLogicJobResult> result;
    private DisruptorRoleLogicJob job;

    public DisruptorRoleLogicJobScheduler(IGameContextHolder gameContextHolder) {
      this.gameContextHolder = gameContextHolder;

      this.result = new NativeArray<DisruptorRoleLogicJobResult>(this.gameContextHolder.MaxAllowedBuddios, Allocator.Persistent);

      this.job.Result = this.result;
    }

    public void Setup(int buddioNum, NativeArray<GameBuddioData> buddios, int ballNum, NativeArray<GameBallData> balls, NativeArray<TeamRoleAssignmentJobResult> teamRoleAssignmentJobResults) {
      this.job.BuddioNum = buddioNum;
      this.job.BuddioDatas = buddios;
      this.job.BallNum = ballNum;
      this.job.BallDatas = balls;
      this.job.TeamRoleAssignmentJobResults = teamRoleAssignmentJobResults;
    }

    public JobHandle ScheduleJob(JobHandle dependency) {
      var jobHandle = this.job.Schedule(this.gameContextHolder.MaxAllowedBuddios, 1, dependency);

      return jobHandle;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DisruptorRoleLogicJobResult GetJobResult(int idx) {
      return this.job.Result[idx];
    }

    public void Dispose() {
      this.result.Dispose();
    }
  }
}