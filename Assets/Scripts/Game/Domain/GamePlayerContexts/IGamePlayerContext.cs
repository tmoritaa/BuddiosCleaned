using System;
using UniRx;

namespace Game.Domain.GamePlayerContexts {
  public interface IGamePlayerContext {
    Side Side { get; }
    ReactiveProperty<int> Score { get; }

    void AddScore(int ptsScored);
    void ResetScore();
  }
}