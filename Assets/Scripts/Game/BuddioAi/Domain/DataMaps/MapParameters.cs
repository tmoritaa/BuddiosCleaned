using System;
using UnityEngine;

namespace Game.BuddioAi.Domain.DataMaps {
  [Serializable]
  public readonly struct MapParameters {
    public readonly Vector2 StageSize;
    public readonly Vector2Int GridNum;

    public Vector2 GridSize => this.StageSize / this.GridNum;

    public Vector2 GridOrigin2dWorldPos => (this.GridNum / 2) * this.GridSize * -1;

    public Vector2 GridCenterOrigin2dWorldPos => this.GridOrigin2dWorldPos + this.GridSize / 2f;

    public MapParameters(Vector2 stageSize, Vector2Int gridNum) {
      this.StageSize = stageSize;
      this.GridNum = gridNum;
    }

    public Vector2 GridCoordinatesTo2dCenteredWorldPos(Vector2Int coord) {
      return this.GridCenterOrigin2dWorldPos + new Vector2(coord.x * this.GridSize.x, coord.y * this.GridSize.y);
    }

    public Vector2Int WorldPosToGridCoordinates(Vector3 worldPos) {
      var worldPos2d = new Vector2(worldPos.x, worldPos.z);

      var adjustedWorldPos2d = worldPos2d - this.GridOrigin2dWorldPos;

      return new Vector2Int(Mathf.FloorToInt(adjustedWorldPos2d.x / this.GridSize.x), Mathf.FloorToInt(adjustedWorldPos2d.y / this.GridSize.y));
    }
  }
}