using System.Collections.Generic;

namespace Game.Domain {
  public interface IEntityHolder<T> where T : IEntityIdHolder {
    List<T> Entities { get; }
    T GetEntityWithId(int id);
  }
}