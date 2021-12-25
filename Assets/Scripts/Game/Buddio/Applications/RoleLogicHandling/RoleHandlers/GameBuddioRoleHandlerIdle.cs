using Game.Buddio.Domain;
using Game.BuddioAi.Jobs.RoleLogic;
using UnityEngine;

namespace Game.Buddio.Applications.RoleLogicHandling.RoleHandlers {
  public class GameBuddioRoleHandlerIdle : IGameBuddioRoleHandler {
    private readonly GameBuddio buddio;

    public GameBuddioRoleHandlerIdle(GameBuddio buddio) {
      this.buddio = buddio;
    }

    public GameBuddioRole Role => GameBuddioRole.Idle;

    public void Handle(RoleLogicResolutionJobResult jobResult) {
      this.buddio.SetMoveDir(Vector3.zero);
    }
  }
}