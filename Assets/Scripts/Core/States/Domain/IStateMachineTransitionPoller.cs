using System;

namespace Core.States.Domain {
  public interface IStateMachineTransitionPoller : IDisposable {
    void StartPolling(StateMachine stateMachine);
    void StopPolling();
  }
}
