namespace Core.States.Domain {
  public class SelfPollingStateMachine : StateMachine {
    private readonly IStateMachineTransitionPoller transitionPoller;

    public SelfPollingStateMachine(IGameState startState, IStateMachineTransitionPoller transitionPoller) : base(startState)
      => this.transitionPoller = transitionPoller;

    public override void Start() {
      this.transitionPoller.StartPolling(this);

      base.Start();
    }

    public override void Stop() {
      this.transitionPoller.StopPolling();

      base.Stop();
    }
  }
}
