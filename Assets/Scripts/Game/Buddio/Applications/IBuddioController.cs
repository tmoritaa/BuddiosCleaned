using UnityEngine;

namespace Game.Buddio.Applications {
  public interface IBuddioController {
    void Face(Vector3 dir);
    void Move(Vector3 dir, float speed);
    void Reset();
  }
}