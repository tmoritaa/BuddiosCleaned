using System;
using Game.Buddio.Domain;
using Unity.Collections;

namespace Game.BuddioAi.Jobs.TeamRoleAssignment.Domain {
  public struct TeamRoleAssignmentJobResult {
    public readonly GameBuddioRole Role;

    public TeamRoleAssignmentJobResult(GameBuddioRole role) {
      this.Role = role;
    }
  }
}