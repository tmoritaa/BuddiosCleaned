using BuddioCore.Domain;
using UnityEngine;

namespace Game.BuddioAi.Domain {
  public static class JobLogicHelper {
    public static bool IsBuddioInAttackRange(Vector3 srcBuddioPos, Vector3 srcBuddioFacingDir, BuddioStats srcBuddioStats, Vector3 tgtBuddioPos) {
      var targetVec = tgtBuddioPos - srcBuddioPos;

      float sqrDist = targetVec.sqrMagnitude;
      float angle = Vector3.Angle(srcBuddioFacingDir, targetVec);

      return sqrDist <= (srcBuddioStats.AttackRange * srcBuddioStats.AttackRange) && angle <= srcBuddioStats.AttackArc;
    }

    public static (Vector3 pos, Vector3 vel) ProjectOneTimeStep(Vector3 lastFramePos, Vector3 lastFrameVel, float drag, float timeStep, Vector3 gravity, Vector2 stageSize) {
      float dragPerTimeStep = (1.0f - drag * timeStep);
      Vector3 gravityPerTimeStep = gravity * timeStep;

      Vector2 halfStageSize = stageSize / 2f;

      Vector3 curFrameVel = lastFrameVel + gravityPerTimeStep;
      curFrameVel *= dragPerTimeStep;

      Vector3 velForTimeStep = curFrameVel * timeStep;

      Vector3 curFramePos = lastFramePos + velForTimeStep;

      // Account for bounce on vertical wall
      if (curFramePos.x >= halfStageSize.x || curFramePos.x <= -halfStageSize.x) {
        curFrameVel.x *= -1;
      }

      // Account for bounce on horizontal wall
      if (curFramePos.z >= halfStageSize.y || curFramePos.z <= -halfStageSize.y) {
        curFrameVel.z *= -1;
      }

      if (curFramePos.y < 0) {
        curFramePos.y = 0;
      }

      return (curFramePos, curFrameVel);
    }
  }
}