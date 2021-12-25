using System;
using System.Runtime.CompilerServices;
using Game.BuddioAi.Domain.DataMaps;
using Game.BuddioAi.Domain.EntityData;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Game.BuddioAi.Jobs.InfluenceMapCalculation.Domain {
  public class InfluenceMapCalculationJobScheduler : IDisposable {
    private InfluenceMapCalculationJobResult result;
    private InfluenceMapCalculationJob job;

    public InfluenceMapCalculationJobScheduler(InfluenceMap influenceMap) {
      var mapParameters = influenceMap.Parameters;
      Vector2Int gridNum = mapParameters.GridNum;

      var mapGrid = new NativeArray<float>(gridNum.x * gridNum.y, Allocator.Persistent);

      this.result = new InfluenceMapCalculationJobResult(mapGrid, gridNum);

      this.job.MapParameters = mapParameters;
      this.job.Result = this.result;
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
    public InfluenceMapCalculationJobResult GetJobResult() {
      return this.result;
    }

    public void Dispose() {
      this.result.Dispose();
    }
  }
}