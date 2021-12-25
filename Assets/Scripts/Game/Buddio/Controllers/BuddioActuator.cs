using UnityEngine;
using Zenject;

namespace Game.Buddio.Controllers {
  public class BuddioActuator : MonoBehaviour {
    [Inject] private readonly Rigidbody body = null;

    public void Move(Vector3 dir, float speed) {
      this.body.velocity = dir.normalized * speed;
    }

    public void Face(Vector3 dir) {
      var angle = Vector3.SignedAngle(Vector3.forward, dir.normalized, Vector3.up);
      this.transform.rotation = Quaternion.Euler(0, angle, 0);
    }
  }
}
