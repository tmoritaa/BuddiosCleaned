using Unity.Collections;
using Zenject;

namespace Game.BuddioAi.Domain.DataMaps {
  public abstract class DataMap<T> : IInitializable where T : struct {
    private T[] grid;

    public readonly MapParameters Parameters;

    public T this[int x, int y] => this.grid[y * this.Parameters.GridNum.x + x];

    protected DataMap(MapParameters parameters) {
      this.Parameters = parameters;
    }

    public void Initialize() {
      this.grid = new T[this.Parameters.GridNum.x * this.Parameters.GridNum.y];
    }

    public void UpdateValues(NativeArray<T> mapGrid) {
      for (int i = 0; i < this.grid.Length; ++i) {
        this.grid[i] = mapGrid[i];
      }
    }
  }
}