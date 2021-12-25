using Zenject;

namespace Game.BuddioAi.Domain.DataMaps {
  public class OccupiedMap : DataMap<OccupiedMapCell> {
    public OccupiedMap([Inject(Id = "occupied")]MapParameters parameters) : base(parameters) {
    }
  }
}