using System;
using Game.BuddioAi.Domain.EntityData;
using Game.Domain;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Zenject;

namespace Game.BuddioAi.Jobs.BallFuturePosProjection.Domain {
  public class BallFuturePosProjectionJobScheduler : IDisposable {
    public const int FUTURE_PROJECTION_COUNT = 10;
    public const float PROJECTION_DURATION_IN_SECS = 1f;

    private readonly IGameContextHolder gameContextHolder;

    private BallFutureProjectionJobResult results;
    private BallFuturePosProjectionJob job;

    public BallFuturePosProjectionJobScheduler(IGameContextHolder gameContextHolder, [Inject(Id = "StageSize")] Vector2 stageSize) {
      this.gameContextHolder = gameContextHolder;

      this.job.Gravity = Physics.gravity;
      this.job.ProjectionCount = FUTURE_PROJECTION_COUNT;
      this.job.ProjectionDurationInSecs = PROJECTION_DURATION_IN_SECS;
      this.job.StageSize = stageSize;

      this.results = new BallFutureProjectionJobResult(
          new NativeArray<Vector3>(FUTURE_PROJECTION_COUNT * this.gameContextHolder.MaxAllowedBalls, Allocator.Persistent),
          FUTURE_PROJECTION_COUNT
        );

      this.job.JobResult = this.results;
    }

    public BallFutureProjectionJobResult Results => this.results;

    public void Setup(int ballNum, NativeArray<GameBallData> balls) {
      this.job.BallNum = ballNum;
      this.job.Balls = balls;
    }

    public JobHandle ScheduleJob() {
      var jobHandle = this.job.Schedule();

      return jobHandle;
    }

    public void Dispose() {
      this.results.Dispose();
    }
  }
}