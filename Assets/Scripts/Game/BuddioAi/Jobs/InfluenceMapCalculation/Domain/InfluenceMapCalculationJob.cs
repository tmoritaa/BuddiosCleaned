using Game.BuddioAi.Domain;
using Game.BuddioAi.Domain.DataMaps;
using Game.BuddioAi.Domain.EntityData;
using Game.Domain;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Game.BuddioAi.Jobs.InfluenceMapCalculation.Domain {
  public struct InfluenceMapCalculationJob : IJob {
    [ReadOnly]
    public NativeArray<GameBuddioData> BuddioDatas;

    [ReadOnly]
    public int BuddioNum;

    [ReadOnly]
    public MapParameters MapParameters;

    public InfluenceMapCalculationJobResult Result;

    public void Execute() {
      var gridNum = this.MapParameters.GridNum;

      for (int y = 0; y < gridNum.y; ++y) {
        for (int x = 0; x < gridNum.x; ++x) {
          this.Result.SetGridValue(x, y, 0);
        }
      }

      const float BASE_ABS_INFLUENCE = 100f;
      for (int i = 0; i < this.BuddioNum; ++i) {
        var buddio = this.BuddioDatas[i];

        var baseInfluence = buddio.Side == Side.Player ? BASE_ABS_INFLUENCE : -BASE_ABS_INFLUENCE;

        for (int y = 0; y < gridNum.y; ++y) {
          for (int x = 0; x < gridNum.x; ++x) {
            var worldPos2d = this.MapParameters.GridCoordinatesTo2dCenteredWorldPos(new Vector2Int(x, y));
            var dist = buddio.Pos - new Vector3(worldPos2d.x, 0, worldPos2d.y);
            var influenceAtCoord = baseInfluence / (dist.magnitude + 1f); // TODO (PERF): should be sqrMagnitude, but w/e for now. Maybe tweak later

            var newInfluenceVal = this.Result.GetGridValue(x, y) + influenceAtCoord;
            this.Result.SetGridValue(x, y, newInfluenceVal);
          }
        }
      }

      var maxAbsInfluence = BASE_ABS_INFLUENCE;
      for (int y = 0; y < gridNum.y; ++y) {
        for (int x = 0; x < gridNum.x; ++x) {
          var val = this.Result.GetGridValue(x, y);

          val = Mathf.Clamp01((val + maxAbsInfluence) / (maxAbsInfluence * 1.5f));

          this.Result.SetGridValue(x, y, val);
        }
      }
    }
  }
}