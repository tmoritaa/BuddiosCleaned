using System.Collections.Generic;
using Game.Buddio.Domain;
using Game.BuddioAi.Jobs.RoleLogic;
using Zenject;

namespace Game.Buddio.Applications.RoleLogicHandling {
  public class GameBuddioRoleHandlerDelegator : ITickable {
    private readonly GameBuddio buddio;
    private readonly Dictionary<GameBuddioRole, IGameBuddioRoleHandler> roleHandlers = new Dictionary<GameBuddioRole, IGameBuddioRoleHandler>();

    private RoleLogicResolutionJobResult curJobResult;

    public GameBuddioRoleHandlerDelegator(GameBuddio gameBuddio, IGameBuddioRoleHandler[] roleHandlers) {
      this.buddio = gameBuddio;

      foreach (var roleHandler in roleHandlers) {
        this.roleHandlers.Add(roleHandler.Role, roleHandler);
      }
    }

    public void UpdateRole(GameBuddioRole role, RoleLogicResolutionJobResult jobResult) {
      this.buddio.SetRole(role);
      this.curJobResult = jobResult;

      this.PerformOneHandleIteration();
    }

    public void Tick() {
      this.PerformOneHandleIteration();
    }

    private void PerformOneHandleIteration() {
      if (!this.buddio.InHitStun) {
        this.roleHandlers[this.buddio.Role].Handle(this.curJobResult);
      }
    }
  }
}