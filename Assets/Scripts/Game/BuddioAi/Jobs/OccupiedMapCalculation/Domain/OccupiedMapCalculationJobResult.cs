using System;
using Game.BuddioAi.Domain.DataMaps;
using Unity.Collections;
using UnityEngine;

namespace Game.BuddioAi.Jobs.OccupiedMapCalculation.Domain {
  public struct OccupiedMapCalculationJobResult : IDisposable {
    public NativeArray<OccupiedMapCell> mapGrid;

    private readonly Vector2Int gridNum;

    public OccupiedMapCalculationJobResult(NativeArray<OccupiedMapCell> mapGrid, Vector2Int gridNum) {
      this.mapGrid = mapGrid;
      this.gridNum = gridNum;
    }

    public void SetGridValue(int x, int y, int flagIdx, bool occupied) {
      var idx = this.Index(x, y);

      var mapGridCell = this.mapGrid[idx];
      mapGridCell.SetOccupiedFlagAtIdx(flagIdx, occupied);
      this.mapGrid[idx] = mapGridCell;
    }

    public bool GetGridValueForFlagIdx(int x, int y, int flagIdx) {
      return this.mapGrid[this.Index(x, y)].IsOccupiedByIdx(flagIdx);
    }

    public bool GetGridValueForAnyOtherFlagIdx(int x, int y, int flagIdx) {
      return this.mapGrid[this.Index(x, y)].IsOccupiedByAnyOtherIdx(flagIdx);
    }

    private int Index(int x, int y) {
      return this.gridNum.x * y + x;
    }

    public void Dispose() {
      this.mapGrid.Dispose();
    }
  }
}