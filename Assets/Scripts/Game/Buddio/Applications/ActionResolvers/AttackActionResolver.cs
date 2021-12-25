using System.Threading;
using Core.ActionResolvers;
using Cysharp.Threading.Tasks;
using Game.Buddio.Domain;
using Game.BuddioAi.Domain;
using Game.Domain;
using UnityEngine;

namespace Game.Buddio.Applications.ActionResolvers {
  public class AttackActionResolver : AsyncActionResolver {
    private readonly GameBuddio gameBuddio;
    private readonly IGameContextHolder gameContextHolder;

    private int targetBuddioId = -1;

    public AttackActionResolver(GameBuddio gameBuddio, IGameContextHolder gameContextHolder) {
      this.gameBuddio = gameBuddio;
      this.gameContextHolder = gameContextHolder;
    }

    public void SetupForRun(int targetBuddioId) {
      this.targetBuddioId = targetBuddioId;
    }

    public override void Run() {
      base.Run();

      this.gameBuddio.SetState(GameBuddioStateType.Attacking);
      this.gameBuddio.SetMoveDir(Vector3.zero);

      this.PerformOp(this.gameBuddio, this.targetBuddioId, this.tokenSource.Token).Forget();
    }

    protected override void Cleanup() {
      this.targetBuddioId = -1;
    }

    private async UniTaskVoid PerformOp(GameBuddio buddio, int targetBuddioId, CancellationToken cancelToken) {
      await UniTask.SwitchToMainThread(PlayerLoopTiming.FixedUpdate);

      var oppBuddio = this.gameContextHolder.Buddios.GetEntityWithId(targetBuddioId);

      var targetVec = oppBuddio.Buddio.Pos - buddio.Pos;

      buddio.SetFacingDir(targetVec.normalized);

      bool inRange = this.IsOppBuddioInRange(buddio, oppBuddio.Buddio);

      if (inRange) {
        Debug.Log($"Attacking Buddio {this.targetBuddioId}");
        oppBuddio.Damaged(targetVec);
      }

      await UniTask.Delay(1000, cancellationToken: cancelToken); // TODO: later switch to scriptable object

      buddio.SetState(GameBuddioStateType.Idle);

      this.SignalComplete();
    }

    private bool IsOppBuddioInRange(GameBuddio buddio, IReadOnlyGameBuddio oppBuddio) {
      return JobLogicHelper.IsBuddioInAttackRange(buddio.Pos, buddio.FacingDir, buddio.Stats, oppBuddio.Pos);
    }
  }
}