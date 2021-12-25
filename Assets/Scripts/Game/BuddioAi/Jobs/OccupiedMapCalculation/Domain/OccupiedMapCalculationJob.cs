using Game.BuddioAi.Domain.DataMaps;
using Game.BuddioAi.Domain.EntityData;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Game.BuddioAi.Jobs.OccupiedMapCalculation.Domain {
  public struct OccupiedMapCalculationJob : IJob {
    [ReadOnly]
    public NativeArray<GameBuddioData> BuddioDatas;

    [ReadOnly]
    public int BuddioNum;

    [ReadOnly]
    public MapParameters MapParameters;

    public OccupiedMapCalculationJobResult Result;

    public void Execute() {
      var gridNum = this.MapParameters.GridNum;

      for (int y = 0; y < gridNum.y; ++y) {
        for (int x = 0; x < gridNum.x; ++x) {
          for (int i = 0; i < this.BuddioNum; ++i) {
            this.Result.SetGridValue(x, y, i, false);
          }
        }
      }

      for (int i = 0; i < this.BuddioNum; ++i) {
        var buddio = this.BuddioDatas[i];

        var gridSize = this.MapParameters.GridSize;
        Vector2Int posCoord = this.MapParameters.WorldPosToGridCoordinates(buddio.Pos);

        const int RADIUS_EXTRA_BUFFER = 1;
        int radiusMaxReach = Mathf.CeilToInt(Mathf.Max(gridSize.x / buddio.BoundsRadius, gridSize.y / buddio.BoundsRadius)) + RADIUS_EXTRA_BUFFER;

        for (int y = Mathf.Max(posCoord.y - radiusMaxReach, 0); y <= Mathf.Min(posCoord.y + radiusMaxReach, gridNum.y - 1); ++y) {
          for (int x = Mathf.Max(posCoord.x - radiusMaxReach, 0); x <= Mathf.Min(posCoord.x + radiusMaxReach, gridNum.x - 1); ++x) {
            this.Result.SetGridValue(x, y, i, true);
          }
        }
      }
    }
  }
}