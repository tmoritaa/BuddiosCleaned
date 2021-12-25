using Game.BuddioAi.Jobs.RoleLogic;
using Game.Domain;
using UnityEngine;

namespace Game.Buddio.Domain {
  public interface IGameBuddioFacade : IEntityIdHolder {
    IReadOnlyGameBuddio Buddio { get; }
    float CatchAreaRadius { get; }

    void Damaged(Vector3 hitDir);
    void TryUpdateRoleWithJobResult(GameBuddioRole role, RoleLogicResolutionJobResult jobResult);
  }
}