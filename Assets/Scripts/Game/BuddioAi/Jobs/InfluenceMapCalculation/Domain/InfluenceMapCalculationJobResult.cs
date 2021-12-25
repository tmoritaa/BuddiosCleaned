using System;
using Unity.Collections;
using UnityEngine;

namespace Game.BuddioAi.Jobs.InfluenceMapCalculation.Domain {
  public struct InfluenceMapCalculationJobResult : IDisposable {
    public NativeArray<float> mapGrid;

    private readonly Vector2Int gridNum;

    public InfluenceMapCalculationJobResult(NativeArray<float> mapGrid, Vector2Int gridNum) {
      this.mapGrid = mapGrid;
      this.gridNum = gridNum;
    }

    public void SetGridValue(int x, int y, float value) {
      var idx = this.Index(x, y);
      this.mapGrid[idx] = value;
    }

    public float GetGridValue(int x, int y) {
      return this.mapGrid[this.Index(x, y)];
    }

    private int Index(int x, int y) {
      return this.gridNum.x * y + x;
    }

    public void Dispose() {
      this.mapGrid.Dispose();
    }
  }
}