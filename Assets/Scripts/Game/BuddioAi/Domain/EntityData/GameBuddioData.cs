using BuddioCore.Domain;
using Game.Domain;
using UnityEngine;

namespace Game.BuddioAi.Domain.EntityData {
  public readonly struct GameBuddioData {
    public readonly int Id;
    public readonly Side Side;
    public readonly Vector3 Pos;
    public readonly Vector3 FacingDir;
    public readonly BuddioStats BuddioStats;
    public readonly float BoundsRadius;
    public readonly bool IsActing;
    public readonly bool HasBall;
    public readonly bool CanBeAttacked;
    public readonly Vector3 ThrowPivotPos;

    public GameBuddioData(int id, Side side, Vector3 pos, Vector3 facingDir, BuddioStats buddioStats, float boundRadius, bool isActing, bool hasBall, bool canBeAttacked, Vector3 throwPivotPos) {
      this.Id = id;
      this.Side = side;
      this.Pos = pos;
      this.FacingDir = facingDir;
      this.BuddioStats = buddioStats;
      this.BoundsRadius = boundRadius;
      this.IsActing = isActing;
      this.HasBall = hasBall;
      this.CanBeAttacked = canBeAttacked;
      this.ThrowPivotPos = throwPivotPos;
    }
  }
}