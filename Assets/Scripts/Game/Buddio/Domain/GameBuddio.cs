using System;
using BuddioCore.Domain;
using Game.Domain;
using Game.GameDebug;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Buddio.Domain {
  public class GameBuddio : IReadOnlyGameBuddio {
    private readonly Transform transform;
    private readonly Transform throwPivot;
    private readonly Transform dropPivot;

    public GameBuddio(int id, Side side, BuddioStats buddioStats, Transform transform, [Inject(Id = "throw_pivot")]Transform throwPivot, [Inject(Id = "drop_pivot")]Transform dropPivot) {
      this.Id = id;
      this.Side = side;
      this.Stats = buddioStats;
      this.transform = transform;
      this.throwPivot = throwPivot;
      this.dropPivot = dropPivot;

      this.MoveDir = Vector3.zero;
      this.FacingDir = Vector3.back;
    }

    private Subject<Unit> caughtSubject = new Subject<Unit>();
    private Subject<Unit> uncaughtSubject = new Subject<Unit>();

    public IObservable<Unit> OnCaught => this.caughtSubject;
    public IObservable<Unit> OnUncaught => this.uncaughtSubject;

    public int Id { get; }
    public Side Side { get; }
    public BuddioStats Stats { get; } // TODO: later probably should remove access to this, since stats could get modified
    public GameBuddioStateType StateType { get; private set; }

    public Transform ThrowPivot => this.throwPivot;
    public Transform DropPivot => this.dropPivot;

    public Vector3 Pos => this.transform.position;
    public Vector3 MoveDir { get; private set; }
    public Vector3 FacingDir { get; private set; }
    public GameBuddioRole Role { get; private set; }
    public Vector3 ThrowPivotPos => this.throwPivot.position;

    public float CurMoveSpeed => this.StateType != GameBuddioStateType.HitStun ? this.Stats.Speed : 0.25f; // TODO: later change to scriptable object

    public bool Acting => this.StateType != GameBuddioStateType.Idle;
    public bool InHitStun => this.StateType == GameBuddioStateType.HitStun;

    public int? CaughtBallId { get; private set; } = null;
    public bool HasBall => this.CaughtBallId.HasValue;
    public bool CanBeAttacked => this.StateType != GameBuddioStateType.HitStun;

    public void SetMoveDir(Vector3 moveDir) {
      this.MoveDir = moveDir;
    }

    public void SetFacingDir(Vector3 facingDir) {
      this.FacingDir = facingDir;
    }

    public void SetState(GameBuddioStateType requestedState) {
      this.StateType = requestedState;
    }

    public void SetRole(GameBuddioRole role) {
      this.Role = role;
    }

    public void SetBallCaught(int ballId) {
      this.CaughtBallId = ballId;
      this.caughtSubject.OnNext(Unit.Default);
    }

    public void ResetBallCaught() {
      this.CaughtBallId = null;
      this.uncaughtSubject.OnNext(Unit.Default);
    }

    public void Reset(Vector3 startPos) {
      this.transform.position = startPos;
      this.CaughtBallId = null;
      this.StateType = GameBuddioStateType.Idle;
      this.MoveDir = Vector3.zero;
      this.FacingDir = new Vector3(Mathf.Sign((Vector3.zero - startPos).x), 0, 0);
      this.Role = GameBuddioRole.Idle;
    }
  }
}