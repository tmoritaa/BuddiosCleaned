using System;

using UniRx;

namespace Domain {
  public interface ITimer : IDisposable {
    int MilliSecondsRemaining { get; }

    IObservable<int> OnTimerUpdated { get; }

    IObservable<Unit> OnComplete { get; }

    void Start(int ms);
  }
}