using System;

namespace Core.States.Domain {
  public class StateMachine {
    private readonly IGameState startState = null;

    protected IGameState curState;

    public bool Running { get; private set; }

    public StateMachine(IGameState startState) {
      this.startState = startState;
    }

    public virtual void Start() {
      this.curState = this.startState;

      this.Running = true;

      this.curState.OnEnter();
    }

    public virtual void Stop() {
      this.Running = false;

      this.curState = null;
    }

    public virtual bool TransitionIfPossible() {
      if (!this.Running) {
        throw new InvalidOperationException("GameStateTracker instructed to try transition before starting");
      }

      bool transitioned = false;
      if (this.curState.CanTransition()) {
        var nextState = this.curState.GetStateToTransition();
        this.curState.OnExit();

        this.curState = nextState;
        this.curState.OnEnter();

        transitioned = true;
      }

      return transitioned;
    }
  }
}
