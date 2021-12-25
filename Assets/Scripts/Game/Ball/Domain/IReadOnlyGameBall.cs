using System;
using UniRx;
using UnityEngine;

namespace Game.Ball.Domain {
  public interface IReadOnlyGameBall {
    IObservable<Unit> OnCaught { get; }
    IObservable<Unit> OnUncaught { get; }

    bool Caught { get; }
    Vector3 Pos { get; }
    Vector3 Vel { get; }
    float Drag { get; }
    float Mass { get; }
  }
}