using System.Threading;
using Core.ActionResolvers;
using Cysharp.Threading.Tasks;
using Game.Ball.Domain;
using Game.Buddio.Domain;
using UnityEngine;

namespace Game.Buddio.Applications.ActionResolvers {
  public class CatchActionResolver : AsyncActionResolver {
    private readonly GameBuddio gameBuddio;

    private IBallFacade ballFacade;

    public CatchActionResolver(GameBuddio gameBuddio) {
      this.gameBuddio = gameBuddio;
    }

    public void SetupForRun(IBallFacade ballFacade) {
      this.ballFacade = ballFacade;
    }

    public override void Run() {
      base.Run();

      this.gameBuddio.SetState(GameBuddioStateType.Catching);
      this.gameBuddio.SetMoveDir(Vector3.zero);

      this.PerformOp(this.gameBuddio, this.ballFacade, this.tokenSource.Token).Forget();
    }

    protected override void Cleanup() {
      this.ballFacade = null;
    }

    private async UniTaskVoid PerformOp(GameBuddio buddio, IBallFacade ballFacade, CancellationToken cancelToken) {
      await UniTask.SwitchToMainThread(PlayerLoopTiming.FixedUpdate);

      buddio.SetBallCaught(ballFacade.Id);
      ballFacade.Catch();

      await UniTask.Delay(100, cancellationToken: cancelToken); // TODO: later change to be based on buddio status

      buddio.SetState(GameBuddioStateType.Idle);
      this.SignalComplete();
    }
  }
}