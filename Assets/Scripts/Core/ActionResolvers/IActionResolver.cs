using System;
using UniRx;

namespace Core.ActionResolvers {
  public interface IActionResolver {
    IObservable<Unit> OnComplete { get; }

    void Run();
    void Cancel();
  }
}