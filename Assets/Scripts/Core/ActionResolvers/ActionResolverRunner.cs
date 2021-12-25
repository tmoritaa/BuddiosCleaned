using System;
using UniRx;

namespace Core.ActionResolvers {
  public abstract class ActionResolverRunner {
    protected readonly ActionResolverHolder actionResolverHolder;

    private IActionResolver curRunningActionResolver = null;

    public bool HasActionResolving => this.curRunningActionResolver != null;

    protected ActionResolverRunner(ActionResolverHolder actionResolverHolder) {
      this.actionResolverHolder = actionResolverHolder;
    }

    public void CancelRunningActionResolver() {
      this.curRunningActionResolver?.Cancel();
      this.curRunningActionResolver = null;
    }

    protected void RunActionResolver(IActionResolver actionResolver) {
      if (this.curRunningActionResolver != null) {
        throw new NotSupportedException($"Trying to perform action {actionResolver.GetType()} when action {this.curRunningActionResolver.GetType()} is already running");
      }

      this.curRunningActionResolver = actionResolver;

      this.curRunningActionResolver.OnComplete.Subscribe(_ => this.curRunningActionResolver = null);

      this.curRunningActionResolver.Run();
    }
  }
}