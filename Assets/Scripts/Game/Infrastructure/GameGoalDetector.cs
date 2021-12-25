using System;
using Game.Applications.Events;
using Game.Ball.Domain;
using Game.Domain;
using UnityEngine;
using Zenject;

namespace Game.Infrastructure {
  public class GameGoalDetector : MonoBehaviour {
    [SerializeField] private Side side = Side.Player;
    [SerializeField] private int ptAdded = 0;

    [Inject] private readonly SignalBus signalBus = null;

    private void OnTriggerEnter(Collider other) {
      var ballFacade = other.attachedRigidbody.GetComponent<IBallFacade>();

      this.HandleBallFacadeCollision(ballFacade);
    }

    private void HandleBallFacadeCollision(IBallFacade ballFacade) {
      if (ballFacade != null) {
        this.signalBus.Fire<TeamScoredEvent>(new TeamScoredEvent(this.side, this.ptAdded));
      }
    }
  }
}