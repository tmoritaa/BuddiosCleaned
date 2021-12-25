using Game.Buddio.Domain;
using Game.BuddioAi.Domain;
using Game.BuddioAi.Domain.DataMaps;
using Game.BuddioAi.Domain.EntityData;
using Game.BuddioAi.Jobs.OccupiedMapCalculation.Domain;
using Game.BuddioAi.Jobs.TeamRoleAssignment.Domain;
using Game.Domain;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Game.BuddioAi.Jobs.RoleLogic.ScorerRoleLogic.Domain {
  public struct ScorerRoleLogicJob : IJob {
    [ReadOnly]
    public bool DebugOn;

    [ReadOnly]
    public NativeArray<GameBuddioData> BuddioDatas;

    [ReadOnly]
    public int BuddioNum;

    [ReadOnly]
    public NativeArray<GameBallData> BallDatas;

    [ReadOnly]
    public int BallNum;

    [ReadOnly]
    public Vector3 Gravity;

    [ReadOnly]
    public int ProjectionCount;

    [ReadOnly]
    public float ProjectionDurationInSecs;

    [ReadOnly]
    public int MaxSearchAngle;

    [ReadOnly]
    public int SearchAngleStep;

    [ReadOnly]
    public NativeArray<TeamRoleAssignmentJobResult> TeamRoleAssignmentJobResults;

    [ReadOnly]
    public MapParameters MapParameters;

    [ReadOnly]
    public OccupiedMapCalculationJobResult OccupiedMapJobResults;

    public NativeArray<ScorerRoleLogicJobResult> Result;

    public NativeArray<ScorerRoleLogicJobDebugResult> DebugCheckedAnglesResult;

    // TODO: put back to parallel later, but leave way to keep non-parallel execution debug output
    public void Execute() {
      for (int i = 0; i < this.BuddioNum; ++i) {
        this.Execute(i);
      }
    }

    public void Execute(int index) {
      if (index >= this.BuddioNum || this.BallNum == 0) {
        return;
      }

      var role = this.TeamRoleAssignmentJobResults[index].Role;
      if (role != GameBuddioRole.Scorer) {
        return;
      }

      var buddio = this.BuddioDatas[index];
      if (buddio.IsActing) {
        return;
      }

      var ballData = this.BallDatas[0];

      float startingVelMag = buddio.BuddioStats.ThrowStr / ballData.Mass;
      float drag = ballData.Drag;
      float timeStep = this.ProjectionDurationInSecs / this.ProjectionCount;

      Vector2 playerSideDir = buddio.Side == Side.Player ? Vector2.right : Vector2.left;

      // TODO: later change to be passed into job
      const float GoalLineAbsX = 11;

      var debugStartIndex = 0;
      if (this.DebugOn) {
        var maxDebugOutputPerBuddio = (2 * this.MaxSearchAngle) / this.SearchAngleStep;
        debugStartIndex = index * maxDebugOutputPerBuddio;
        for (int i = debugStartIndex; i < debugStartIndex + maxDebugOutputPerBuddio; ++i) {
          this.DebugCheckedAnglesResult[i] = new ScorerRoleLogicJobDebugResult();
        }
      }

      bool reachedGoal = false;
      Vector3 reachedGoalThrowDir = Vector3.zero;
      Vector3 reachedGoalThrowPivot = Vector3.zero;
      int count = debugStartIndex;

      for (int a = 0; a < 2 * this.MaxSearchAngle / this.SearchAngleStep; ++a) {
        float sign = a % 2 == 0 ? 1 : -1;
        float angle = ((int)(a / 2)) * this.SearchAngleStep * sign;

        this.PrintDebugLog($"Doing search for angle {angle}");

        Vector2 throwDir2d = playerSideDir.Rotate(angle * Mathfs.Deg2Rad);
        Vector3 throwDir = new Vector3(throwDir2d.x, 0, throwDir2d.y).normalized;

        var throwPivotVec = (buddio.ThrowPivotPos - buddio.Pos);
        var angleDiffFromFacing = Mathfs.SignedAngle(new Vector2(buddio.FacingDir.x, buddio.FacingDir.z), throwDir2d);
        var throwPivotVec2d = new Vector2(throwPivotVec.x, throwPivotVec.z).Rotate(angleDiffFromFacing);
        throwPivotVec = new Vector3(throwPivotVec2d.x, throwPivotVec.y, throwPivotVec2d.y);

        var testingThrowPivotPos = buddio.Pos + throwPivotVec;
        Vector3 lastFramePos = testingThrowPivotPos;
        Vector3 lastFrameVel = throwDir * startingVelMag;

        this.PrintDebugLog($"Starting Pos {lastFramePos} ThrowPivotPos {testingThrowPivotPos} ThrowDir {throwDir} Starting Vel {lastFrameVel}");

        for (int i = 0; i < this.ProjectionCount; ++i) {
          var (curFramePos, curFrameVel) = JobLogicHelper.ProjectOneTimeStep(lastFramePos, lastFrameVel, drag, timeStep, this.Gravity, this.MapParameters.StageSize);

          var posInGridCoords = this.MapParameters.WorldPosToGridCoordinates(curFramePos);
          posInGridCoords.Clamp(Vector2Int.zero, this.MapParameters.GridNum - new Vector2Int(1, 1));

          var occupied = this.OccupiedMapJobResults.GetGridValueForAnyOtherFlagIdx(posInGridCoords.x, posInGridCoords.y, index);

          this.PrintDebugLog($"Pos {curFramePos} Coord {posInGridCoords} is occupied?={occupied}");

          if (occupied) {
            break;
          }

          if ((buddio.Side == Side.Player && curFramePos.x >= GoalLineAbsX) || (buddio.Side == Side.Enemy && curFramePos.x <= -GoalLineAbsX)) {
            reachedGoal = true;
            break;
          }

          lastFrameVel = curFrameVel;
          lastFramePos = curFramePos;
        }

        if (this.DebugOn) {
          // NOTE: debug stuff
          // w field used to signify whether throw dir has reached goal
          Vector4 debugThrowDir = throwDir;
          debugThrowDir.w = reachedGoal ? 1 : 0;
          this.DebugCheckedAnglesResult[count] = new ScorerRoleLogicJobDebugResult(testingThrowPivotPos, debugThrowDir);
          count += 1;
        }


        if (reachedGoal) {
          reachedGoalThrowDir = throwDir;
          reachedGoalThrowPivot = testingThrowPivotPos;
          break;
        }
      }

      this.Result[index] = new ScorerRoleLogicJobResult(reachedGoal, reachedGoalThrowPivot + reachedGoalThrowDir * 10);
    }

    private void PrintDebugLog(string log) {
      if (this.DebugOn) {
        Debug.Log(log);
      }
    }
  }
}