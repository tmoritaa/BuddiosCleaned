using System.Collections.Generic;
using UnityEngine;

namespace Game.Domain {
  public class EntityContainer<T> : IEntityHolder<T> where T : IEntityIdHolder {
    private readonly List<T> entities = new List<T>();
    private readonly Dictionary<int, T> entityDict = new Dictionary<int, T>();

    public List<T> Entities => this.entities;

    public void AddEntity(T entity) {
      this.entities.Add(entity);
      this.entityDict.Add(entity.Id, entity);
    }

    public T GetEntityWithId(int id) {
      if (!this.entityDict.ContainsKey(id)) {
        Debug.LogError($"ID {id} doesn't exist in dictionary");
        return default(T);
      }

      return this.entityDict[id];
    }
  }
}