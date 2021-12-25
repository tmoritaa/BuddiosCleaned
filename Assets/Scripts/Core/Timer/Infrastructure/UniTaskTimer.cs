using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using Zenject;

using Domain;
using Domain.Factories;
using UnityEngine;

namespace Infrastructure {
  public class UniTaskTimer : ITimer {
    private readonly Subject<Unit> completeSubject = new Subject<Unit>();
    private readonly Subject<int> timerUpdatedSubject = new Subject<int>();

    public IObservable<int> OnTimerUpdated => this.timerUpdatedSubject;

    public IObservable<Unit> OnComplete => this.completeSubject;

    public int MilliSecondsRemaining { get; private set; }

    private CancellationTokenSource curTokenSource;

    public void Start(int ms) {
      if (this.curTokenSource != null) {
        throw new InvalidOperationException("Timer start called when already started. Not supported.");
      }

      this.curTokenSource = new CancellationTokenSource();

      this.delayFor(ms, this.curTokenSource.Token).Forget();
    }

    public void Dispose() {
      this.curTokenSource?.Cancel();
      this.curTokenSource?.Dispose();
      this.curTokenSource = null;

      this.completeSubject.Dispose();
      this.timerUpdatedSubject.Dispose();
    }

    private async UniTaskVoid delayFor(int ms, CancellationToken token) {
      this.MilliSecondsRemaining = ms;

      while (this.MilliSecondsRemaining > 0 && !token.IsCancellationRequested) {
        await UniTask.Yield();

        this.MilliSecondsRemaining = Math.Max(this.MilliSecondsRemaining - (int)(Time.deltaTime * 1000), 0);

        if (!token.IsCancellationRequested) {
          this.timerUpdatedSubject.OnNext(this.MilliSecondsRemaining);
        }
      }

      if (!token.IsCancellationRequested) {
        this.completeSubject.OnCompleted();
      }
    }

    public class Factory : PlaceholderFactory<ITimer>, ITimerFactory {
    }
  }
}