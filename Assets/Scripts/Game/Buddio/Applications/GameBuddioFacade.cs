using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Applications.Events;
using Game.Ball.Domain;
using Game.Buddio.Applications.ActionResolvers;
using Game.Buddio.Applications.RoleLogicHandling;
using Game.Buddio.Domain;
using Game.BuddioAi.Jobs.RoleLogic;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Buddio.Applications {
  public class GameBuddioFacade : MonoBehaviour, IGameBuddioFacade, IInitializable, IDisposable {
    [Inject] private readonly Vector3 startPos = Vector3.zero; // TODO: feels a bit dumb that this is stored here the whole time. Maybe a use case later?

    [Inject] private readonly GameBuddio buddio = null;
    [Inject] private readonly IBuddioController controller = null;
    [Inject] private readonly IBuddioEventListener eventListener = null;
    [Inject] private readonly GameBuddioActionResolverRunner actionResolverRunner = null;
    [Inject] private readonly GameBuddioRoleHandlerDelegator roleHandlerDelegator = null;

    [Inject(Id = "catch_area")] private readonly CapsuleCollider catchAreaCollider = null;

    [Inject] private readonly SignalBus signalBus = null;

    private bool IsStopped => this.cancellationTokenSource == null;

    private CancellationTokenSource cancellationTokenSource;

    public int Id => this.Buddio.Id;

    public float CatchAreaRadius => this.catchAreaCollider.radius;

    public IReadOnlyGameBuddio Buddio => this.buddio;

    public void Initialize() {
      this.buddio.Reset(this.startPos);

      this.eventListener.OnCatch
        .Where(ball => !this.buddio.Acting && !ball.GameBall.Caught && !this.IsStopped)
        .Subscribe(this.CatchBall);

      this.signalBus.Subscribe<GameplayStartEvent>(this.OnStartForGameplay);
      this.signalBus.Subscribe<GameplayStopEvent>(this.OnStopForGameplay);
      this.signalBus.Subscribe<ResetForNewRoundEvent>(this.OnResetForNewRound);
    }

    public void Dispose() {
      this.cancellationTokenSource?.Dispose();
      this.cancellationTokenSource = null;

      this.signalBus.Unsubscribe<GameplayStartEvent>(this.OnStartForGameplay);
      this.signalBus.Unsubscribe<GameplayStopEvent>(this.OnStopForGameplay);
      this.signalBus.Unsubscribe<ResetForNewRoundEvent>(this.OnResetForNewRound);
    }

    public void TryUpdateRoleWithJobResult(GameBuddioRole role, RoleLogicResolutionJobResult jobResult) {
      var canUpdate = !this.buddio.Acting;

      if (canUpdate) {
        this.roleHandlerDelegator.UpdateRole(role, jobResult);
      }
    }

    public void Damaged(Vector3 hitDir) {
      Debug.Log($"{this.buddio.Id} damaged");

      this.actionResolverRunner.CancelRunningActionResolver();
      this.actionResolverRunner.EnterHitStun(hitDir, this.buddio.DropPivot);
    }

    // TODO: probably move to Domain level
    public void Throw(Vector3 targetPos) {
      if (!this.buddio.Acting && !this.actionResolverRunner.HasActionResolving) {
        this.actionResolverRunner.PerformThrow(targetPos, this.buddio.ThrowPivot);
      }
    }

    public void Pass(int targetBuddioId) {
      if (!this.actionResolverRunner.HasActionResolving) {
        this.actionResolverRunner.PerformPass(targetBuddioId, this.buddio.ThrowPivot);
      }
    }

    // TODO: probably move to Domain level
    public void Move(Vector3 moveDir) {
      if (!this.buddio.Acting) {
        buddio.SetMoveDir(moveDir);

        if (moveDir != Vector3.zero) {
          this.buddio.SetFacingDir(moveDir);
        }
      }
    }

    public void Attack(int targetBuddioId) {
      if (!this.actionResolverRunner.HasActionResolving) {
        this.actionResolverRunner.PerformAttack(targetBuddioId);
      }
    }

    private void CatchBall(IBallFacade ball) {
      if (!this.actionResolverRunner.HasActionResolving) {
        this.actionResolverRunner.PerformCatch(ball);
      }
    }

    private async UniTaskVoid OnFixedUpdate(CancellationToken cancellationToken) {
      await UniTask.SwitchToMainThread(PlayerLoopTiming.FixedUpdate);

      while (!cancellationToken.IsCancellationRequested) {
        this.controller.Face(this.buddio.FacingDir);
        this.controller.Move(this.Buddio.MoveDir, this.Buddio.Stats.Speed);

        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
      }
    }

    private void OnStartForGameplay(GameplayStartEvent evt) {
      this.cancellationTokenSource = new CancellationTokenSource();
      this.OnFixedUpdate(this.cancellationTokenSource.Token).Forget();
    }

    private void OnStopForGameplay(GameplayStopEvent evt) {
      this.actionResolverRunner.CancelRunningActionResolver();

      this.cancellationTokenSource?.Cancel();
      this.cancellationTokenSource?.Dispose();
      this.cancellationTokenSource = null;
    }

    private void OnResetForNewRound(ResetForNewRoundEvent evt) {
      this.buddio.Reset(this.startPos);

      this.controller.Reset();
      this.controller.Face(this.buddio.FacingDir);

      this.controller.Move(this.buddio.MoveDir, this.buddio.CurMoveSpeed);
    }
  }
}