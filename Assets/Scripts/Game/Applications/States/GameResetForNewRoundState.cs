using System.Threading;
using Core.States.Domain;
using Cysharp.Threading.Tasks;
using Game.Applications.Events;
using UnityEngine;
using Zenject;

namespace Game.Applications.States {
  public class GameResetForNewRoundState : BaseGameState {
    private readonly SignalBus signalBus;

    private CancellationTokenSource tokenSource;

    public GameResetForNewRoundState(SignalBus signalBus) {
      this.signalBus = signalBus;
    }

    public bool Complete { get; private set; }

    public override void Dispose() {
      this.tokenSource?.Cancel();
      this.tokenSource?.Dispose();
      this.tokenSource = null;
    }

    protected override void onEnter() {
      this.Complete = false;

      this.tokenSource = new CancellationTokenSource();

      this.OnLateFixedUpdate(this.tokenSource.Token).Forget();
    }

    protected override void onExit() {
      this.tokenSource?.Dispose();
      this.tokenSource = null;
    }

    private async UniTaskVoid OnLateFixedUpdate(CancellationToken cancelToken) {
      await UniTask.SwitchToMainThread(PlayerLoopTiming.FixedUpdate);

      await UniTask.DelayFrame(2, PlayerLoopTiming.FixedUpdate, cancelToken);

      this.signalBus.Fire<ResetForNewRoundEvent>();

      await UniTask.Yield(PlayerLoopTiming.LastUpdate, cancelToken);

      this.Complete = true;
    }
  }
}