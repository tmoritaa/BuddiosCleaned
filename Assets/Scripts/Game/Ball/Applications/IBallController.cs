using UnityEngine;

namespace Game.Ball.Applications {
  public interface IBallController {
    void Push(Vector3 startPos, Vector3 dir, float strength);
    void Reset();
  }
}