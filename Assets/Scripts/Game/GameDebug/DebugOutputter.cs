using System.Collections.Generic;
using Game.Ball.Domain;
using Game.Buddio.Domain;
using Game.BuddioAi.Domain;
using Game.BuddioAi.Domain.DataMaps;
using Game.BuddioAi.Jobs.RoleLogic.ScorerRoleLogic.Domain;
using Game.Domain;
using Game.Installers;
using Unity.Collections;
using UnityEngine;
using Zenject;

namespace Game.GameDebug {
  [ExecuteInEditMode]
  public class DebugOutputter : MonoBehaviour, IDebugOptions {
    [SerializeField]
    private bool debugOutputOn = true;

    [SerializeField]
    private bool showInfluenceMapOutput = false;

    [SerializeField]
    private bool showOccupiedMapOutput = false;

    [SerializeField]
    private int occupiedMapIdxFilter = -1;

    [SerializeField]
    private bool showCoordinateConversionOutput = false;

    [SerializeField]
    private bool showScorerRoleDebugOutput = false;

    [SerializeField]
    private GameScriptableObjectInstaller gameScriptableObjectInstaller = null; // TODO: this is awful but hey this is debug code so w/e

    [Inject]
    private InfluenceMap influenceMap = null;

    [Inject]
    private OccupiedMap occupiedMap = null;

    [Inject]
    private IGameContextHolder gameContextHolder = null;

    public bool ShowScorerRoleDebugOutput => this.showScorerRoleDebugOutput;

    public void DrawBallProjection(IBallFacade ballFacade, List<Vector3> futureProjections) {
      var lastPos = ballFacade.GameBall.Pos;
      for (int i = 0; i < futureProjections.Count; ++i) {
        var ratio = (float) i / futureProjections.Count;
        var color = Color.green * (1.0f - ratio) + Color.red * ratio;
        Debug.DrawLine(lastPos, futureProjections[i], color, 0.01f);

        lastPos = futureProjections[i];
      }
    }

    public void DrawScorerBuddioDebugOutput(NativeArray<ScorerRoleLogicJobDebugResult> debugOutput) {
      if (!this.ShowScorerRoleDebugOutput) {
        return;
      }

      var outputPerBuddio = (2 * ScorerRoleLogicJobScheduler.MAX_SEARCH_ANGLE) / ScorerRoleLogicJobScheduler.SEARCH_ANGLE_STEP;
      for (int i = 0; i < this.gameContextHolder.Buddios.Entities.Count; ++i) {
        var buddio = this.gameContextHolder.Buddios.Entities[i];

        if (buddio.Buddio.Role != GameBuddioRole.Scorer) {
          continue;
        }

        var startIndex = outputPerBuddio * i;
        for (int idx = startIndex; idx < startIndex + outputPerBuddio; ++idx) {
          var result = debugOutput[idx];
          var pivotPos = result.PivotPos;
          Vector3 throwDir = result.ThrowDir;
          Color color = result.ThrowDir.w > 0 ? Color.green : Color.red;
          Debug.DrawLine(pivotPos, pivotPos + throwDir * 10f, color, 0.01f);
        }
      }
    }

    private void DrawInfluenceMap(MapParameters mapParameters) {
      var origColor = Gizmos.color;

      Gizmos.color = Color.green;

      const float yOffset = 0.5f;

      var initialOffset = mapParameters.GridCenterOrigin2dWorldPos;

      for (int y = 0; y < mapParameters.GridNum.y; ++y) {
        for (int x = 0; x < mapParameters.GridNum.x; ++x) {
          var center = initialOffset + new Vector2(x, y) * mapParameters.GridSize;

          var convertedCenter = new Vector3(center.x, yOffset, center.y);
          var convertedGridSize = new Vector3(mapParameters.GridSize.x, 0, mapParameters.GridSize.y);

          var value = this.influenceMap[x, y];
          Color color = Color.red * (1 - value) + Color.green * (value);
          color.a = 0.5f;
          Gizmos.color = color;

          Gizmos.DrawCube(convertedCenter, convertedGridSize);
        }
      }

      Gizmos.color = origColor;
    }

    private void DrawOccupiedMap(MapParameters mapParameters) {
      var origColor = Gizmos.color;

      Gizmos.color = Color.green;

      const float yOffset = 0.5f;

      var initialOffset = mapParameters.GridCenterOrigin2dWorldPos;

      for (int y = 0; y < mapParameters.GridNum.y; ++y) {
        for (int x = 0; x < mapParameters.GridNum.x; ++x) {
          var center = initialOffset + new Vector2(x, y) * mapParameters.GridSize;

          var convertedCenter = new Vector3(center.x, yOffset, center.y);
          var convertedGridSize = new Vector3(mapParameters.GridSize.x, 0, mapParameters.GridSize.y);

          if (this.occupiedMap != null) {
            var value = this.occupiedMap[x, y];

            var occupied = this.occupiedMapIdxFilter < 0 ? value.IsOccupied : value.IsOccupiedByAnyOtherIdx(this.occupiedMapIdxFilter);

            if (occupied) {
              Color color = Color.red;
              color.a = 0.5f;
              Gizmos.color = color;

              Gizmos.DrawCube(convertedCenter, convertedGridSize);
            }
          }
        }
      }

      Gizmos.color = origColor;
    }

    private void DrawMapBoundsAndGrid(MapParameters mapParameters) {
      var origColor = Gizmos.color;

      Gizmos.color = Color.green;

      const float yOffset = 0.5f;

      Gizmos.DrawWireCube(new Vector3(0, yOffset, 0), new Vector3(mapParameters.StageSize.x, 0, mapParameters.StageSize.y));

      var initialOffset = mapParameters.GridCenterOrigin2dWorldPos;

      var newColor = Color.green;
      newColor.a = 0.1f;
      Gizmos.color = newColor;
      for (int y = 0; y < mapParameters.GridNum.y; ++y) {
        for (int x = 0; x < mapParameters.GridNum.x; ++x) {
          var center = initialOffset + new Vector2(x, y) * mapParameters.GridSize;

          var convertedCenter = new Vector3(center.x, yOffset, center.y);
          var convertedGridSize = new Vector3(mapParameters.GridSize.x, 0, mapParameters.GridSize.y);

          Gizmos.DrawWireCube(convertedCenter, convertedGridSize);
        }
      }

      Gizmos.color = origColor;
    }

    private void PrintCoordinateConversionOutput() {
      foreach (var buddio in this.gameContextHolder.Buddios.Entities) {
        Vector2Int gridCoordinate = this.influenceMap.Parameters.WorldPosToGridCoordinates(buddio.Buddio.Pos);

        var worldPos = this.influenceMap.Parameters.GridCoordinatesTo2dCenteredWorldPos(gridCoordinate);
        Debug.Log($"Buddio at pos {buddio.Buddio.Pos} is at coordinate {gridCoordinate} and worldPos ({worldPos.x:F5},{worldPos.y:F5})");
      }
    }

    private void OnDrawGizmos() {
      if (!this.debugOutputOn) {
        return;
      }

      if (this.showInfluenceMapOutput) {
        var mapParameters = this.gameScriptableObjectInstaller.InfluenceMapParameters;
        this.DrawMapBoundsAndGrid(mapParameters);

        if (this.influenceMap != null) {
          this.DrawInfluenceMap(mapParameters);
        }
      }

      if (this.showOccupiedMapOutput) {
        var mapParameters = this.gameScriptableObjectInstaller.OccupiedMapParameters;
        this.DrawMapBoundsAndGrid(mapParameters);

        if (this.occupiedMap != null) {
          this.DrawOccupiedMap(mapParameters);
        }

      }

      if (this.showCoordinateConversionOutput && this.gameContextHolder != null && this.influenceMap != null) {
        this.PrintCoordinateConversionOutput();
      }
    }
  }
}