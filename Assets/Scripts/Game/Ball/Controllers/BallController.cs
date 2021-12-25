using Game.Ball.Applications;
using Game.Ball.Domain;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Ball.Controllers {
  public class BallController : IBallController, IInitializable {
    private readonly IBallFacade ballFacade;
    private readonly Transform transform;
    private readonly Rigidbody rigidbody;

    public BallController(IBallFacade ballFacade, Transform transform, Rigidbody rigidbody) {
      this.ballFacade = ballFacade;
      this.transform = transform;
      this.rigidbody = rigidbody;
    }

    public void Initialize() {
      this.Reset();

      this.ballFacade.GameBall.OnCaught.Subscribe(_ => {
        this.transform.gameObject.SetActive(false);
      });

      this.ballFacade.GameBall.OnUncaught.Subscribe(_ => {
        this.transform.gameObject.SetActive(true);
      });
    }

    public void Push(Vector3 startPos, Vector3 dir, float strength) {
      this.transform.position = startPos;
      this.rigidbody.AddForce(dir * strength, ForceMode.Impulse);
    }

    public void Reset() {
      this.transform.gameObject.SetActive(true);
    }
  }
}