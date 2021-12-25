using System;
using Game.BuddioAi.Domain.DataMaps;
using UnityEngine;
using Zenject;

namespace Game.Installers {
  [CreateAssetMenu(fileName = "GameScriptableObjectInstaller", menuName = "Installers/GameScriptableObjectInstaller")]
  public class GameScriptableObjectInstaller : ScriptableObjectInstaller<GameScriptableObjectInstaller> {
    [SerializeField] private GameSettings gameSettings = new GameSettings();

    public MapParameters InfluenceMapParameters => new MapParameters(this.gameSettings.StageSize, this.gameSettings.InfluenceMapGridNum);
    public MapParameters OccupiedMapParameters => new MapParameters(this.gameSettings.StageSize, this.gameSettings.OccupiedMapGridNum);

    public override void InstallBindings() {
      this.Container.BindInstance<Vector2>(this.gameSettings.StageSize).WithId("StageSize");

      this.Container.BindInstance<MapParameters>(this.InfluenceMapParameters).WithId("influence");
      this.Container.BindInstance<MapParameters>(this.OccupiedMapParameters).WithId("occupied");
    }

    [Serializable]
    public struct GameSettings {
      [SerializeField] private Vector2 stageSize;

      public Vector2 StageSize => this.stageSize;

      [SerializeField] private Vector2Int influenceMapGridNum;
      public Vector2Int InfluenceMapGridNum => this.influenceMapGridNum;

      [SerializeField] private Vector2Int occupiedMapGridNum;
      public Vector2Int OccupiedMapGridNum => this.occupiedMapGridNum;

      public GameSettings(Vector2 stageSize, Vector2Int influenceMapGridNum, Vector2Int occupiedMapGridNum) {
        this.stageSize = stageSize;
        this.influenceMapGridNum = influenceMapGridNum;
        this.occupiedMapGridNum = occupiedMapGridNum;
      }
    }
  }
}
