using Game.Domain;

namespace Game.Applications.Events {
  public class TeamScoredEvent {
    public readonly Side Side;
    public readonly int PtsScored;

    public TeamScoredEvent(Side side, int ptsScored) {
      this.Side = side;
      this.PtsScored = ptsScored;
    }
  }
}