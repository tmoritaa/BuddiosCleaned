using Core.ActionResolvers;
using Game.Ball.Domain;
using UnityEngine;

namespace Game.Buddio.Applications.ActionResolvers {
  public class GameBuddioActionResolverRunner : ActionResolverRunner {

    public GameBuddioActionResolverRunner(ActionResolverHolder actionResolverHolder) : base(actionResolverHolder) {
    }

    public void PerformPass(int targetBuddioId, Transform throwPivot) {
      var actionResolver = this.actionResolverHolder.GetActionResolver<PassActionResolver>();
      actionResolver.SetupForRun(targetBuddioId, throwPivot);

      this.RunActionResolver(actionResolver);
    }

    public void PerformThrow(Vector3 targetPos, Transform throwPivot) {
      var actionResolver = this.actionResolverHolder.GetActionResolver<ThrowActionResolver>();
      actionResolver.SetupForRun(targetPos, throwPivot);

      this.RunActionResolver(actionResolver);
    }

    public void PerformCatch(IBallFacade ballFacade) {
      var actionResolver = this.actionResolverHolder.GetActionResolver<CatchActionResolver>();
      actionResolver.SetupForRun(ballFacade);

      this.RunActionResolver(actionResolver);
    }

    public void PerformAttack(int targetBuddioId) {
      var actionResolver = this.actionResolverHolder.GetActionResolver<AttackActionResolver>();
      actionResolver.SetupForRun(targetBuddioId);

      this.RunActionResolver(actionResolver);
    }

    public void EnterHitStun(Vector3 hitDir, Transform dropPivot) {
      var actionResolver = this.actionResolverHolder.GetActionResolver<HitStunActionResolver>();
      actionResolver.SetupForRun(hitDir, dropPivot);

      this.RunActionResolver(actionResolver);
    }
  }
}