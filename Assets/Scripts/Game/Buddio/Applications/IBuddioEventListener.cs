using System;
using Game.Ball.Domain;

namespace Game.Buddio.Applications {
  public interface IBuddioEventListener {
    IObservable<IBallFacade> OnCatch { get; }
  }
}