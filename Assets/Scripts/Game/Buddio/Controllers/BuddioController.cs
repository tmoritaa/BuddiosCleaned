using Game.Buddio.Applications;
using Game.Buddio.Domain;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Buddio.Controllers {
  public class BuddioController : IBuddioController, IInitializable {
    private readonly IGameBuddioFacade buddioFacade;
    private readonly BuddioActuator actuator;

    [Inject(Id = "ball_possess")] private readonly Transform ballPossess = null;

    public BuddioController(IGameBuddioFacade buddioFacade, BuddioActuator actuator) {
      this.buddioFacade = buddioFacade;
      this.actuator = actuator;
    }

    public void Initialize() {
      this.buddioFacade.Buddio.OnCaught.Subscribe(_ => this.OnCaught());
      this.buddioFacade.Buddio.OnUncaught.Subscribe(_ => this.OnUncaught());

      this.Reset();
    }

    public void Face(Vector3 dir) {
      this.actuator.Face(dir);
    }

    public void Move(Vector3 dir, float speed) {
      this.actuator.Move(dir, speed);
    }

    public void Reset() {
      this.ballPossess.gameObject.SetActive(false);
    }

    private void OnCaught() {
      this.ballPossess.gameObject.SetActive(true);
    }

    private void OnUncaught() {
      this.ballPossess.gameObject.SetActive(false);
    }
  }
}