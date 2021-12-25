using System.Threading;
using Core.ActionResolvers;
using Cysharp.Threading.Tasks;
using Game.Buddio.Domain;
using Game.Domain;
using UnityEngine;

namespace Game.Buddio.Applications.ActionResolvers {
  public class HitStunActionResolver : AsyncActionResolver {
    private readonly IGameContextHolder gameContextHolder;
    private readonly GameBuddio gameBuddio;

    private Vector3 hitDir;
    private Transform dropPivot;

    public HitStunActionResolver(IGameContextHolder gameContextHolder, GameBuddio gameBuddio) {
      this.gameContextHolder = gameContextHolder;
      this.gameBuddio = gameBuddio;
    }

    public void SetupForRun(Vector3 hitDir, Transform dropPivot) {
      this.hitDir = hitDir;
      this.dropPivot = dropPivot;
    }

    public override void Run() {
      base.Run();

      this.gameBuddio.SetState(GameBuddioStateType.HitStun);
      this.gameBuddio.SetMoveDir(Vector3.zero);

      this.PerformOp(this.gameBuddio, this.hitDir, this.dropPivot, this.tokenSource.Token).Forget();
    }

    protected override void Cleanup() {
      this.hitDir = Vector3.zero;
      this.dropPivot = null;
    }

    private async UniTaskVoid PerformOp(GameBuddio buddio, Vector3 hitDir, Transform dropPivot, CancellationToken cancelToken) {
      await UniTask.SwitchToMainThread(PlayerLoopTiming.FixedUpdate);

      buddio.SetFacingDir(-hitDir);
      buddio.SetMoveDir(hitDir);

      if (this.gameBuddio.HasBall) {
        var ballId = this.gameBuddio.CaughtBallId.Value;
        buddio.ResetBallCaught();
        var ballFacade = this.gameContextHolder.Balls.GetEntityWithId(ballId);
        ballFacade.Push(dropPivot.position, hitDir, 1); // TODO: later switch to scriptable object
      }

      Debug.Log("In Hitstun");
      await UniTask.Delay(1000, cancellationToken: cancelToken); // TODO: later switch to scriptable object

      buddio.SetState(GameBuddioStateType.Idle);

      this.SignalComplete();
    }
  }
}