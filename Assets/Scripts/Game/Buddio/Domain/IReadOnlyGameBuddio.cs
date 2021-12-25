using System;
using BuddioCore.Domain;
using Game.Domain;
using UniRx;
using UnityEngine;

namespace Game.Buddio.Domain {
  public interface IReadOnlyGameBuddio {
    IObservable<Unit> OnCaught { get; }
    IObservable<Unit> OnUncaught { get; }

    int Id { get; }
    Side Side { get; }
    BuddioStats Stats { get; }

    Vector3 Pos { get; }
    Vector3 MoveDir { get; }
    Vector3 FacingDir { get; }

    bool Acting { get; }
    bool HasBall { get; }
    bool CanBeAttacked { get; }

    GameBuddioRole Role { get; }

    Vector3 ThrowPivotPos { get; }
  }
}