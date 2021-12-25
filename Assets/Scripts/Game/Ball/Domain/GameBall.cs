using System;
using UniRx;
using UnityEngine;

namespace Game.Ball.Domain {
  public class GameBall : IReadOnlyGameBall, IDisposable {
    public readonly int Id;
    private readonly Transform transform; // TODO: shouldn't have reference
    private readonly Rigidbody rigidbody; // TODO: shouldn't have reference

    private readonly Subject<Unit> caughtSubject = new Subject<Unit>();
    private readonly Subject<Unit> uncaughtSubject = new Subject<Unit>();

    public IObservable<Unit> OnCaught => this.caughtSubject;
    public IObservable<Unit> OnUncaught => this.uncaughtSubject;

    public bool Caught { get; private set; }

    public GameBall(int id, Transform transform, Rigidbody rigidbody) {
      this.Id = id;
      this.transform = transform;
      this.rigidbody = rigidbody;
    }

    public Vector3 Pos => this.transform.position;
    public Vector3 Vel => this.rigidbody.velocity;
    public float Drag => this.rigidbody.drag;
    public float Mass => this.rigidbody.mass;

    // TODO: this really shouldn't be changing rigidbody velocity directly, and should instead be going through controller.
    public void SetCaught(bool b) {
      this.Caught = b;

      if (this.Caught) {
        this.rigidbody.velocity = Vector3.zero;
        this.caughtSubject.OnNext(Unit.Default);
      } else {
        this.uncaughtSubject.OnNext(Unit.Default);
      }
    }

    // TODO: this really shouldn't be changing rigidbody velocity or position directly, and should instead be going through controller.
    public void Reset(Vector3 startPos) {
      this.rigidbody.velocity = Vector3.zero;
      this.transform.position = startPos;
      this.Caught = false;
    }

    public void Dispose() {
      this.caughtSubject?.Dispose();
    }
  }
}