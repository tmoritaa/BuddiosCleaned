using Game.Buddio.Controllers;
using UnityEngine;
using Zenject;

namespace Game.Buddio.Inputs {
  public class BuddioEventDelegator : MonoBehaviour {
    [SerializeField] private BuddioEventType evntType = BuddioEventType.Catch;

    [Inject] private readonly BuddioEventListener eventListener = null;

    private void OnTriggerStay(Collider other) {
      this.eventListener.PropagateEvent(this.evntType, other);
    }
  }
}