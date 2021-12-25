using System;
using System.Runtime.CompilerServices;
using Game.BuddioAi.Domain;
using Game.BuddioAi.Domain.DataMaps;
using Game.BuddioAi.Domain.EntityData;
using Game.BuddioAi.Jobs.OccupiedMapCalculation.Domain;
using Game.BuddioAi.Jobs.TeamRoleAssignment.Domain;
using Game.Domain;
using Game.GameDebug;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Zenject;

namespace Game.BuddioAi.Jobs.RoleLogic.ScorerRoleLogic.Domain {
  public class ScorerRoleLogicJobScheduler : IDisposable {
    private const int FUTURE_PROJECTION_COUNT = 100;
    private const float PROJECTION_DURATION_IN_SECS = 2f;
    public const int MAX_SEARCH_ANGLE = 40;
    public const int SEARCH_ANGLE_STEP = 10;

    private readonly IGameContextHolder gameContextHolder;
    private readonly IDebugOptions debugOptions;

    private NativeArray<ScorerRoleLogicJobResult> result;
    private ScorerRoleLogicJob job;

    public NativeArray<ScorerRoleLogicJobDebugResult> DebugResult { get; }

    public ScorerRoleLogicJobScheduler(IGameContextHolder gameContextHolder, IDebugOptions debugOptions,  OccupiedMap occupiedMap) {
      this.gameContextHolder = gameContextHolder;
      this.debugOptions = debugOptions;

      this.job.Gravity = Physics.gravity;
      this.job.ProjectionCount = FUTURE_PROJECTION_COUNT;
      this.job.ProjectionDurationInSecs = PROJECTION_DURATION_IN_SECS;
      this.job.MaxSearchAngle = MAX_SEARCH_ANGLE;
      this.job.SearchAngleStep = SEARCH_ANGLE_STEP;
      this.job.MapParameters = occupiedMap.Parameters;

      this.result = new NativeArray<ScorerRoleLogicJobResult>(this.gameContextHolder.MaxAllowedBuddios, Allocator.Persistent);

      this.DebugResult = new NativeArray<ScorerRoleLogicJobDebugResult>((2 * MAX_SEARCH_ANGLE) / SEARCH_ANGLE_STEP * this.gameContextHolder.MaxAllowedBuddios, Allocator.Persistent);

      this.job.Result = this.result;
      this.job.DebugCheckedAnglesResult = this.DebugResult;
    }

    public void Setup(int buddioNum, NativeArray<GameBuddioData> buddios, int ballNum, NativeArray<GameBallData> balls, NativeArray<TeamRoleAssignmentJobResult> teamRoleAssignmentJobResults, OccupiedMapCalculationJobResult occupiedMapJobResults) {
      this.job.BuddioNum = buddioNum;
      this.job.BuddioDatas = buddios;
      this.job.BallNum = ballNum;
      this.job.BallDatas = balls;
      this.job.TeamRoleAssignmentJobResults = teamRoleAssignmentJobResults;
      this.job.OccupiedMapJobResults = occupiedMapJobResults;

      this.job.DebugOn = this.debugOptions.ShowScorerRoleDebugOutput;
    }

    public JobHandle ScheduleJob(JobHandle dependency) {
      var jobHandle = this.job.Schedule(dependency);

      return jobHandle;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScorerRoleLogicJobResult GetJobResult(int idx) {
      return this.job.Result[idx];
    }

    public void Dispose() {
      this.result.Dispose();
      this.DebugResult.Dispose();
    }
  }
}