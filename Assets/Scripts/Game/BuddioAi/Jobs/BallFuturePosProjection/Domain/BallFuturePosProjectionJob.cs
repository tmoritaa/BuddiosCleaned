using Game.BuddioAi.Domain;
using Game.BuddioAi.Domain.EntityData;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Game.BuddioAi.Jobs.BallFuturePosProjection.Domain {
  public struct BallFuturePosProjectionJob : IJob {
    [ReadOnly]
    public NativeArray<GameBallData> Balls;

    [ReadOnly]
    public int BallNum;

    [ReadOnly]
    public Vector3 Gravity;

    [ReadOnly]
    public int ProjectionCount;

    [ReadOnly]
    public float ProjectionDurationInSecs;

    [ReadOnly]
    public Vector2 StageSize;

    public BallFutureProjectionJobResult JobResult;

    public void Execute() {
      for (int i = 0; i < this.BallNum; ++i) {
        this.ExecuteForParallel(i);
      }
    }

    private void ExecuteForParallel(int index) {
      if (index >= this.BallNum) {
        return;
      }

      var ballData = this.Balls[index];

      float drag = ballData.Drag;
      float timeStep = this.ProjectionDurationInSecs / this.ProjectionCount;

      Vector3 lastFrameVel = ballData.Vel;
      Vector3 lastFramePos = ballData.Pos;
      for (int i = 0; i < this.ProjectionCount; ++i) {
        var (curFramePos, curFrameVel) = JobLogicHelper.ProjectOneTimeStep(lastFramePos, lastFrameVel, drag, timeStep, this.Gravity, this.StageSize);

        this.JobResult.SetPosForBall(index, i, curFramePos);

        lastFrameVel = curFrameVel;
        lastFramePos = curFramePos;
      }
    }
  }
}