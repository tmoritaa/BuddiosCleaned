using System.Threading;
using UniRx;

namespace Core.ActionResolvers {
  public abstract class AsyncActionResolver : ActionResolver {
    protected CancellationTokenSource tokenSource;

    public override void Run() {
      base.Run();

      this.tokenSource = new CancellationTokenSource();

      this.OnComplete.Subscribe(_ => this.CancelAndCleanupTokenSource());
    }

    public override void Cancel() {
      base.Cancel();

      this.CancelAndCleanupTokenSource();
    }

    protected override void OnDispose() {
      this.CancelAndCleanupTokenSource();
    }

    private void CancelAndCleanupTokenSource() {
      this.tokenSource?.Cancel();
      this.tokenSource?.Dispose();
      this.tokenSource = null;
    }
  }
}