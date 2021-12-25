namespace Core.States.Domain {
  public interface ITransition {
    IGameState NextGameState { get; }

    bool CanTransition();
  }
}