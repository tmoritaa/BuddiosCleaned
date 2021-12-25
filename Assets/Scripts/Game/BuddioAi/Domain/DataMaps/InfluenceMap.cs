using Zenject;

namespace Game.BuddioAi.Domain.DataMaps {
  public class InfluenceMap : DataMap<float> {
    public InfluenceMap([Inject(Id = "influence")] MapParameters parameters) : base(parameters) {
    }
  }
}