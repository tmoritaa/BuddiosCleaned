using Core.States;
using Core.States.Domain;

namespace Game.Applications.States {
  public class GameStateMachine : SelfPollingStateMachine {
    public GameStateMachine(
      GameSetupState setupState,
      GamePlayState playState,
      GameResetForNewRoundState resetForNewRoundState,
      IStateMachineTransitionPoller transitionPoller) : base(setupState, transitionPoller) {
      setupState.AddTransition(new Transition(playState, () => true));
      playState.AddTransition(new Transition(resetForNewRoundState, () => playState.ProceedToNewRound));
      resetForNewRoundState.AddTransition(new Transition(playState, () => resetForNewRoundState.Complete));
    }
  }
}