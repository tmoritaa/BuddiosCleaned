using System;
using System.Runtime.CompilerServices;
using Game.BuddioAi.Domain.DataMaps;
using Game.BuddioAi.Domain.EntityData;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Game.BuddioAi.Jobs.OccupiedMapCalculation.Domain {
  public class OccupiedMapCalculationJobScheduler : IDisposable {
    public OccupiedMapCalculationJobResult Result { get; }

    private OccupiedMapCalculationJob job;

    public OccupiedMapCalculationJobScheduler(OccupiedMap occupiedMap) {
      var mapParameters = occupiedMap.Parameters;
      Vector2Int gridNum = mapParameters.GridNum;

      var mapGrid = new NativeArray<OccupiedMapCell>(gridNum.x * gridNum.y, Allocator.Persistent);

      this.Result = new OccupiedMapCalculationJobResult(mapGrid, gridNum);

      this.job.MapParameters = mapParameters;
      this.job.Result = this.Result;
    }

    public void Setup(int buddioNum, NativeArray<GameBuddioData> buddios) {
      this.job.BuddioNum = buddioNum;
      this.job.BuddioDatas = buddios;
    }

    public JobHandle ScheduleJob() {
      var jobHandle = this.job.Schedule();

      return jobHandle;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OccupiedMapCalculationJobResult GetJobResult() {
      return this.Result;
    }

    public void Dispose() {
      this.Result.Dispose();
    }
  }
}