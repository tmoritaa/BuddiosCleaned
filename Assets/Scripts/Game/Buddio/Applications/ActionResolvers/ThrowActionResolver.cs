using System.Threading;
using Core.ActionResolvers;
using Cysharp.Threading.Tasks;
using Game.Buddio.Domain;
using Game.Domain;
using UnityEngine;

namespace Game.Buddio.Applications.ActionResolvers {
  public class ThrowActionResolver : AsyncActionResolver {
    private readonly IGameContextHolder gameContextHolder;
    private readonly GameBuddio gameBuddio;

    private Vector3 targetPos;
    private Transform throwPivot;

    public ThrowActionResolver(IGameContextHolder gameContextHolder, GameBuddio gameBuddio) {
      this.gameContextHolder = gameContextHolder;
      this.gameBuddio = gameBuddio;
    }

    public void SetupForRun(Vector3 targetPos, Transform throwPivot) {
      this.targetPos = targetPos;
      this.throwPivot = throwPivot;
    }

    public override void Run() {
      base.Run();

      this.gameBuddio.SetState(GameBuddioStateType.Throwing);
      this.gameBuddio.SetMoveDir(Vector3.zero);

      this.PerformOp(this.gameBuddio, this.targetPos, this.throwPivot, this.tokenSource.Token).Forget();
    }

    protected override void Cleanup() {
      this.targetPos = Vector3.zero;
      this.throwPivot = null;
    }

    private async UniTaskVoid PerformOp(GameBuddio buddio, Vector3 targetPos, Transform throwPivot, CancellationToken cancelToken) {
      await UniTask.SwitchToMainThread(PlayerLoopTiming.FixedUpdate);

      if (!buddio.HasBall) {
        buddio.SetState(GameBuddioStateType.Idle);
        this.SignalComplete();
        return;
      }

      var faceDir = (targetPos - buddio.Pos).normalized;

      // TODO: removing for now to simplify scorer role logic job calculation.
      // Once things are figured out a bit more should fix this up and fix up scorer role logic job calculation
      buddio.SetFacingDir(faceDir.normalized);

      Debug.Log($"Buddio {buddio.Id} winding up for Throw");

      await UniTask.Delay(500, cancellationToken: cancelToken); // TODO: later switch to scriptable object

      Debug.Log($"Buddio {buddio.Id} throwing at pos {targetPos}");
      var ball = this.gameContextHolder.Balls.GetEntityWithId(buddio.CaughtBallId.Value);

      var throwDir = (targetPos - throwPivot.position).normalized;
      ball.Push(throwPivot.position, throwDir, buddio.Stats.ThrowStr);

      await UniTask.Delay(100, cancellationToken: cancelToken); // TODO: later switch to scriptable object;
      Debug.Log($"Buddio {buddio.Id} Finished follow through");

      buddio.ResetBallCaught();
      buddio.SetState(GameBuddioStateType.Idle);

      this.SignalComplete();
    }
  }
}