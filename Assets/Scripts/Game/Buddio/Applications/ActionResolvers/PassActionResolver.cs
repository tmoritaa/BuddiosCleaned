using System.Threading;
using Core.ActionResolvers;
using Cysharp.Threading.Tasks;
using Game.Buddio.Domain;
using Game.Domain;
using UnityEngine;

namespace Game.Buddio.Applications.ActionResolvers {
  public class PassActionResolver : AsyncActionResolver {
    private readonly IGameContextHolder gameContextHolder;
    private readonly GameBuddio gameBuddio;

    private int targetBuddioId;
    private Transform throwPivot;

    public PassActionResolver(IGameContextHolder gameContextHolder, GameBuddio gameBuddio) {
      this.gameContextHolder = gameContextHolder;
      this.gameBuddio = gameBuddio;
    }

    public void SetupForRun(int targetBuddioId, Transform throwPivot) {
      this.targetBuddioId = targetBuddioId;
      this.throwPivot = throwPivot;
    }

    public override void Run() {
      base.Run();

      this.gameBuddio.SetState(GameBuddioStateType.Throwing);
      this.gameBuddio.SetMoveDir(Vector3.zero);

      this.PerformOp(this.gameBuddio, this.targetBuddioId, this.throwPivot, this.tokenSource.Token).Forget();
    }

    protected override void Cleanup() {
      this.targetBuddioId = -1;
      this.throwPivot = null;
    }

    private async UniTaskVoid PerformOp(GameBuddio buddio, int targetBuddioId, Transform throwPivot, CancellationToken cancelToken) {
      await UniTask.SwitchToMainThread(PlayerLoopTiming.FixedUpdate);

      if (!buddio.HasBall) {
        buddio.SetState(GameBuddioStateType.Idle);
        this.SignalComplete();
        return;
      }

      var targetBuddio = this.gameContextHolder.Buddios.GetEntityWithId(targetBuddioId);

      var faceDir = (targetBuddio.Buddio.Pos - buddio.Pos).normalized;

      buddio.SetFacingDir(faceDir);

      Debug.Log($"Buddio {buddio.Id} winding up for Throw");

      await UniTask.Delay(500, cancellationToken: cancelToken); // TODO: later switch to scriptable object

      Debug.Log($"Buddio {buddio.Id} throwing");
      var ball = this.gameContextHolder.Balls.GetEntityWithId(buddio.CaughtBallId.Value);

      var throwDir = (targetBuddio.Buddio.Pos - throwPivot.position).normalized;
      buddio.SetFacingDir(throwDir);
      ball.Push(throwPivot.position, throwDir, buddio.Stats.ThrowStr);

      await UniTask.Delay(100, cancellationToken: cancelToken); // TODO: later switch to scriptable object;
      Debug.Log($"Buddio {buddio.Id} Finished follow through");

      buddio.ResetBallCaught();
      buddio.SetState(GameBuddioStateType.Idle);

      this.SignalComplete();
    }
  }
}