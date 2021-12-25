using System;
using Unity.Collections;
using UnityEngine;

namespace Game.BuddioAi.Jobs.BallFuturePosProjection.Domain {
  public struct BallFutureProjectionJobResult : IDisposable {
    public NativeArray<Vector3> FutureBallPos;

    private int projectionCount;

    public BallFutureProjectionJobResult(NativeArray<Vector3> allocatedArray, int projectionCount) {
      this.FutureBallPos = allocatedArray;
      this.projectionCount = projectionCount;
    }

    public Vector3 GetPosForBall(int ballIdx, int posIdx) {
      return this.FutureBallPos[ballIdx * this.projectionCount + posIdx];
    }

    public void SetPosForBall(int ballIdx, int posIdx, Vector3 newPos) {
      this.FutureBallPos[ballIdx * this.projectionCount + posIdx] = newPos;
    }

    public void Dispose() {
      this.FutureBallPos.Dispose();
    }
  }
}