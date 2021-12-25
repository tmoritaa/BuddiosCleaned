using System;
using UniRx;

namespace Game.Domain.GamePlayerContexts {
  public class GamePlayerContext : IGamePlayerContext {
    public Side Side { get; }

    public ReactiveProperty<int> Score { get; } = new ReactiveProperty<int>(0);

    public GamePlayerContext(Side side) {
      this.Side = side;
    }

    public void AddScore(int scoredPts) {
      this.SetScore(this.Score.Value + scoredPts);
    }

    public void ResetScore() {
      this.SetScore(0);
    }

    private void SetScore(int score) {
      this.Score.Value = score;
    }
  }
}