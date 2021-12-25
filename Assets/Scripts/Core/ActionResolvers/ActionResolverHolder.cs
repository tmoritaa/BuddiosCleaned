using System;
using System.Collections.Generic;

namespace Core.ActionResolvers {
  public class ActionResolverHolder {
    private readonly Dictionary<Type, IActionResolver> actionResolvers = new Dictionary<Type, IActionResolver>();

    public ActionResolverHolder(IActionResolver[] actionResolvers) {
      foreach (var actionResolver in actionResolvers) {
        this.actionResolvers.Add(actionResolver.GetType(), actionResolver);
      }
    }

    public T GetActionResolver<T>() {
      return (T)this.actionResolvers[typeof(T)];
    }
  }
}