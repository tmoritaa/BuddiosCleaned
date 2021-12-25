using System.Linq;
using Core.States.Domain;
using Game.Applications.Events;
using Game.BuddioAi.Applications;
using Game.Domain;
using Game.Domain.GamePlayerContexts;
using UnityEngine;
using Zenject;

namespace Game.Applications.States {
  public class GamePlayState : BaseGameState, ITickable, IFixedTickable {
    private readonly IGameContextHolder gameContextHolder;
    private readonly GamePlayerContextHolder gamePlayerContextHolder;
    private readonly BuddioAiScheduler aiScheduler;
    private readonly SignalBus signalBus;

    private bool exiting = false;

    private bool BlockTicking => !this.IsRunning || this.exiting;

    public GamePlayState(IGameContextHolder contextHolder, GamePlayerContextHolder gamePlayerContextHolder, BuddioAiScheduler aiScheduler, SignalBus signalBus) {
      this.gameContextHolder = contextHolder;
      this.gamePlayerContextHolder = gamePlayerContextHolder;
      this.aiScheduler = aiScheduler;
      this.signalBus = signalBus;
    }

    public bool ProceedToNewRound { get; private set; }

    // TODO: probably optimize later to use UniTask
    public void Tick() {
      if (this.BlockTicking) {
        return;
      }

      if (!this.aiScheduler.ProcessingJob) {
        this.aiScheduler.Schedule();
      }

      if (Input.GetKeyUp(KeyCode.R)) {
        this.ProceedToNewRound = true;
      }

      var ball = this.gameContextHolder.Balls.Entities[0];
      if (Input.GetKeyUp(KeyCode.W)) {
        ball.Push(ball.GameBall.Pos, Vector3.forward, 20f);
      } else if (Input.GetKeyUp(KeyCode.S)) {
        ball.Push(ball.GameBall.Pos, Vector3.back, 20f);
      }

      if (Input.GetKeyUp(KeyCode.A)) {
        ball.Push(ball.GameBall.Pos, Vector3.left, 20f);
      } else if (Input.GetKeyUp(KeyCode.D)) {
        ball.Push(ball.GameBall.Pos, Vector3.right, 20f);
      }
    }

    // TODO: probably optimize later to use UniTask
    public void FixedTick() {
      if (this.BlockTicking) {
        return;
      }

      if (this.aiScheduler.IsJobComplete) {
        this.aiScheduler.ApplyJobResult();
      }
    }

    public override void Dispose() {
      this.signalBus.TryUnsubscribe<TeamScoredEvent>(this.OnTeamScored);
    }

    protected override void onEnter() {
      this.exiting = false;
      this.ProceedToNewRound = false;

      this.signalBus.Subscribe<TeamScoredEvent>(this.OnTeamScored);

      this.signalBus.Fire<GameplayStartEvent>();
    }

    protected override void onExit() {
      this.exiting = true;

      this.signalBus.Unsubscribe<TeamScoredEvent>(this.OnTeamScored);

      this.aiScheduler.WaitForCompleteAndDiscard();
      this.signalBus.Fire<GameplayStopEvent>();
    }

    private void OnTeamScored(TeamScoredEvent evt) {
      if (ProceedToNewRound) {
        return;
      }

      this.gamePlayerContextHolder.GetGamePlayerContext(evt.Side).AddScore(evt.PtsScored);

      this.ProceedToNewRound = true;
    }
  }
}