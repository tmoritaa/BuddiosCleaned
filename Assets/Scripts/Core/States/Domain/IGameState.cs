using System;

namespace Core.States.Domain {
  public interface IGameState : IDisposable {
    void OnEnter();
    void OnExit();

    bool CanTransition();

    IGameState GetStateToTransition();
  }
}
