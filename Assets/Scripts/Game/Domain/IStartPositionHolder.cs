using UnityEngine;

namespace Game.Domain {
  public interface IStartPositionHolder {
    Vector3[] PlayerBuddioStartPos { get; }
    Vector3[] EnemyBuddioStartPos { get; }
    Vector3 BallStartPos { get; }
  }
}