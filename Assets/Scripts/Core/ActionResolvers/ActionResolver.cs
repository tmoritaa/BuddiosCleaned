using System;
using UniRx;

namespace Core.ActionResolvers {
  public abstract class ActionResolver : IActionResolver, IDisposable {
    private readonly Subject<Unit> completeSubject = new Subject<Unit>();

    public IObservable<Unit> OnComplete => this.completeSubject;

    public virtual void Run() {
      this.OnComplete.Subscribe(_ => this.Cleanup());
    }

    public virtual void Cancel() {
      this.Cleanup();
    }

    public void Dispose() {
      this.OnDispose();
      this.completeSubject?.Dispose();
    }

    protected abstract void Cleanup();

    protected virtual void OnDispose() {
    }

    protected void SignalComplete() {
      this.completeSubject.OnNext(Unit.Default);
    }
  }
}