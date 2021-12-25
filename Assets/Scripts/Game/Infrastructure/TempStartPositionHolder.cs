using Game.Domain;
using UnityEngine;

namespace Game.Infrastructure {
  public class TempStartPositionHolder : IStartPositionHolder {
    public Vector3[] PlayerBuddioStartPos { get; } = new[] { new Vector3(-8, 0, 0), new Vector3(-8, 0, 4), new Vector3(-8, 0, -4) };
    public Vector3[] EnemyBuddioStartPos { get; } = new[] { new Vector3(8, 0, 0), new Vector3(8, 0, 4), new Vector3(8, 0, -4) };

    public Vector3 BallStartPos { get; } = Vector3.up;
  }
}