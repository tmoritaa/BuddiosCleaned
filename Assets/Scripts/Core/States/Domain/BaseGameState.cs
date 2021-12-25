using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.States.Domain {
  public abstract class BaseGameState : IGameState {
    private List<ITransition> transitions = new List<ITransition>();

    protected bool IsRunning { get; private set; } = false;

    public void OnEnter() {
      this.IsRunning = true;
      this.onEnter();
    }

    public void OnExit() {
      this.IsRunning = false;
      this.onExit();
    }

    public bool CanTransition() => this.transitions.Any(t => t.CanTransition());

    public IGameState GetStateToTransition() {
      var validTransitions = this.transitions.FindAll(t => t.CanTransition());

      if (validTransitions.Count != 1) {
        throw validTransitions.Count > 1 ?
            new InvalidOperationException("Multiple valid transitions exist") : new InvalidOperationException("No valid transitions exist");
      }

      return validTransitions.First().NextGameState;
    }

    public void AddTransition(ITransition transition) => this.transitions.Add(transition);

    public virtual void Cancel() {}

    public virtual void Dispose() {}

    protected abstract void onEnter();

    protected abstract void onExit();
  }

  public class Transition : ITransition {
    private readonly IGameState state;
    private readonly Func<bool> condition;

    public Transition(IGameState state, Func<bool> condition) => (this.state, this.condition) = (state, condition);

    public IGameState NextGameState => this.state;

    public bool CanTransition() {
      return this.condition();
    }
  }
}
