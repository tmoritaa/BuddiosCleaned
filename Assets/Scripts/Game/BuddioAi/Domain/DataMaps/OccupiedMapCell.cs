namespace Game.BuddioAi.Domain.DataMaps {
  public struct OccupiedMapCell {
    private int occupationFlag;

    public bool IsOccupied => this.occupationFlag > 0;

    public void SetOccupiedFlagAtIdx(int idx, bool occupied) {
      if (occupied) {
        this.occupationFlag |= (0x1 << idx);
      } else {
        this.occupationFlag &= ~(0x1 << idx);
      }
    }

    public bool IsOccupiedByIdx(int idx) {
      return (this.occupationFlag & (0x1 << idx)) > 0;
    }

    public bool IsOccupiedByAnyOtherIdx(int idx) {
      return (this.occupationFlag & ~(0x1 << idx)) > 0;
    }
  }
}