using System.Collections.Generic;
using Game.Domain;
using UnityEngine;

namespace Game.Ball.Domain {
  public interface IBallFacade : IEntityIdHolder {
    IReadOnlyGameBall GameBall { get; }

    void Catch();

    void Push(Vector3 startPos, Vector3 dir, float strength);
  }
}