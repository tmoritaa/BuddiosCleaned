using System;
using Game.Ball.Domain;
using Game.Buddio.Applications;
using UniRx;
using UnityEngine;

namespace Game.Buddio.Controllers {
  public class BuddioEventListener : IBuddioEventListener, IDisposable {
    private readonly Subject<IBallFacade> catchSubject = new Subject<IBallFacade>();

    public IObservable<IBallFacade> OnCatch => this.catchSubject;

    public void PropagateEvent(BuddioEventType eventType, Collider collider) {
      switch (eventType) {
        case BuddioEventType.Catch:
          var ballFacade = collider.attachedRigidbody.GetComponent<IBallFacade>();
          this.catchSubject.OnNext(ballFacade);
          break;
      }
    }

    public void Dispose() {
      this.catchSubject?.Dispose();
    }
  }
}