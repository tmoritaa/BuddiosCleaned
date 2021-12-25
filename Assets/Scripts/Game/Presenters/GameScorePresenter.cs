using Game.Domain;
using Game.Domain.GamePlayerContexts;
using UniRx;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Presenters {
  public class GameScorePresenter : MonoBehaviour, IInitializable {
    [SerializeField] private Side side = Side.Player;

    [Inject] private readonly GamePlayerContextHolder playerContextHolder = null;
    [Inject] private readonly TMP_Text scoreText = null;

    public void Initialize() {
      this.playerContextHolder.GetGamePlayerContext(this.side).Score
        .Subscribe(score => this.scoreText.text = $"{this.side}: {score}");
    }
  }
}