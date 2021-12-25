using System;
using System.Collections.Generic;
using Game.Applications.Events;
using Game.Ball.Domain;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Ball.Applications {
  public class BallFacade : MonoBehaviour, IBallFacade, IInitializable, IDisposable {
    [Inject] private readonly Vector3 initialPos = Vector3.zero; // TODO: feels a bit dumb that this is stored here the whole time. Maybe a use case later?
    [Inject] private readonly GameBall gameBall = null;

    [Inject] private readonly IBallController controller = null;

    [Inject] private readonly SignalBus signalBus = null;

    public int Id => this.gameBall.Id;
    public IReadOnlyGameBall GameBall => this.gameBall;

    public void Initialize() {
      this.gameBall.Reset(this.initialPos);

      this.signalBus.Subscribe<ResetForNewRoundEvent>(this.OnResetForNewRound);
    }

    public void Dispose() {
      this.signalBus.Unsubscribe<ResetForNewRoundEvent>(this.OnResetForNewRound);
    }

    public void Catch() {
      this.gameBall.SetCaught(true);
    }

    public void Push(Vector3 startPos, Vector3 dir, float strength) {
      this.gameBall.SetCaught(false);
      this.controller.Push(startPos, dir, strength);
    }

    private void OnResetForNewRound(ResetForNewRoundEvent evt) {
      this.gameBall.Reset(this.initialPos);
      this.controller.Reset();
    }
  }
}