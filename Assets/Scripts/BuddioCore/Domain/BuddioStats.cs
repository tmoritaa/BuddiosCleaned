using UnityEngine;

namespace BuddioCore.Domain {
  public readonly struct BuddioStats {
    public readonly float Speed;
    public readonly float ThrowStr;
    public readonly float AttackRange;
    public readonly float AttackArc;

    public BuddioStats(float speed, float throwStr, float attackRange, float attackArc) {
      this.Speed = speed;
      this.ThrowStr = throwStr;
      this.AttackRange = attackRange;
      this.AttackArc = attackArc;
    }
  }
}