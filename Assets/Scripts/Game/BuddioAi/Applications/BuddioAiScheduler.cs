using System;
using System.Collections.Generic;
using Game.Ball.Domain;
using Game.Buddio.Domain;
using Game.BuddioAi.Domain;
using Game.BuddioAi.Domain.DataMaps;
using Game.BuddioAi.Domain.EntityData;
using Game.BuddioAi.Jobs.BallFuturePosProjection.Domain;
using Game.BuddioAi.Jobs.InfluenceMapCalculation.Domain;
using Game.BuddioAi.Jobs.OccupiedMapCalculation.Domain;
using Game.BuddioAi.Jobs.RoleLogic;
using Game.BuddioAi.Jobs.RoleLogic.BallPursuerRoleLogic.Domain;
using Game.BuddioAi.Jobs.RoleLogic.DefenderRoleLogic.Domain;
using Game.BuddioAi.Jobs.RoleLogic.DisruptorRoleLogic.Domain;
using Game.BuddioAi.Jobs.RoleLogic.PasserRoleLogic.Domain;
using Game.BuddioAi.Jobs.RoleLogic.ScorerRoleLogic.Domain;
using Game.BuddioAi.Jobs.TeamRoleAssignment.Domain;
using Game.Domain;
using Game.GameDebug;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Game.BuddioAi.Applications {
  public class BuddioAiScheduler : IDisposable {
    private readonly IGameContextHolder gameContextHolder;
    private readonly InfluenceMap influenceMap;
    private readonly OccupiedMap occupiedMap;
    private readonly DebugOutputter debugOutputter;

    private readonly BallFuturePosProjectionJobScheduler ballFuturePosProjectionJobScheduler;
    private readonly InfluenceMapCalculationJobScheduler influenceMapCalculationJobScheduler;
    private readonly OccupiedMapCalculationJobScheduler occupiedMapCalculationJobScheduler;
    private readonly TeamRoleAssignmentJobScheduler teamRoleAssignmentJobScheduler;

    private readonly BallPursuerRoleLogicJobScheduler ballPursuerRoleLogicJobScheduler;
    private readonly DefenderRoleLogicJobScheduler defenderRoleLogicJobScheduler;
    private readonly DisruptorRoleLogicJobScheduler disruptorRoleLogicJobScheduler;
    private readonly PasserRoleLogicJobScheduler passerRoleLogicJobScheduler;
    private readonly ScorerRoleLogicJobScheduler scorerRoleLogicJobScheduler;

    private int curEvaluatingBuddioNum;

    private NativeArray<GameBuddioData> buddiosArr;
    private NativeArray<GameBallData> ballsArr;

    private JobHandle? jobHandle;

    public BuddioAiScheduler(
      IGameContextHolder gameContextHolder,
      InfluenceMap influenceMap,
      OccupiedMap occupiedMap,
      DebugOutputter debugOutputter,
      BallFuturePosProjectionJobScheduler ballFuturePosProjectionJobScheduler,
      InfluenceMapCalculationJobScheduler influenceMapCalculationJobScheduler,
      OccupiedMapCalculationJobScheduler occupiedMapCalculationJobScheduler,
      TeamRoleAssignmentJobScheduler teamRoleAssignmentJobScheduler,
      BallPursuerRoleLogicJobScheduler ballPursuerRoleLogicJobScheduler,
      DefenderRoleLogicJobScheduler defenderRoleLogicJobScheduler,
      DisruptorRoleLogicJobScheduler disruptorRoleLogicJobScheduler,
      PasserRoleLogicJobScheduler passerRoleLogicJobScheduler,
      ScorerRoleLogicJobScheduler scorerRoleLogicJobScheduler) {
      this.gameContextHolder = gameContextHolder;
      this.influenceMap = influenceMap;
      this.occupiedMap = occupiedMap;
      this.debugOutputter = debugOutputter;

      this.ballFuturePosProjectionJobScheduler = ballFuturePosProjectionJobScheduler;
      this.influenceMapCalculationJobScheduler = influenceMapCalculationJobScheduler;
      this.occupiedMapCalculationJobScheduler = occupiedMapCalculationJobScheduler;
      this.teamRoleAssignmentJobScheduler = teamRoleAssignmentJobScheduler;

      this.ballPursuerRoleLogicJobScheduler = ballPursuerRoleLogicJobScheduler;
      this.defenderRoleLogicJobScheduler = defenderRoleLogicJobScheduler;
      this.disruptorRoleLogicJobScheduler = disruptorRoleLogicJobScheduler;
      this.passerRoleLogicJobScheduler = passerRoleLogicJobScheduler;
      this.scorerRoleLogicJobScheduler = scorerRoleLogicJobScheduler;

      this.buddiosArr = new NativeArray<GameBuddioData>(this.gameContextHolder.MaxAllowedBuddios, Allocator.Persistent);
      this.ballsArr = new NativeArray<GameBallData>(this.gameContextHolder.MaxAllowedBalls, Allocator.Persistent);
    }

    public bool ProcessingJob => this.jobHandle.HasValue;

    public bool IsJobComplete => this.jobHandle.HasValue && this.jobHandle.Value.IsCompleted;

    public void Dispose() {
      if (this.jobHandle.HasValue) {
        this.jobHandle.Value.Complete();
      }

      this.buddiosArr.Dispose();
      this.ballsArr.Dispose();
    }

    public void Schedule() {
      this.PrepareJobs();

      var ballFutureProjectionJobHandle = this.ballFuturePosProjectionJobScheduler.ScheduleJob();
      var influenceMapCalculationJobHandle = this.influenceMapCalculationJobScheduler.ScheduleJob();
      var occupiedMapCalculationJobHandle = this.occupiedMapCalculationJobScheduler.ScheduleJob();
      var teamRoleAssignmentJobHandle = this.teamRoleAssignmentJobScheduler.ScheduleJob();

      var prePreWorkHandles = JobHandle.CombineDependencies(ballFutureProjectionJobHandle, influenceMapCalculationJobHandle, occupiedMapCalculationJobHandle);
      var preWorkHandles = JobHandle.CombineDependencies(teamRoleAssignmentJobHandle, prePreWorkHandles);

      var ballPursuerJobHandle = this.ballPursuerRoleLogicJobScheduler.ScheduleJob(preWorkHandles);
      var passerJobHandle = this.passerRoleLogicJobScheduler.ScheduleJob(preWorkHandles);
      var scorerJobHandle = this.scorerRoleLogicJobScheduler.ScheduleJob(preWorkHandles);
      var defenderJobHandle = this.defenderRoleLogicJobScheduler.ScheduleJob(preWorkHandles);
      var disruptorJobHandle = this.disruptorRoleLogicJobScheduler.ScheduleJob(preWorkHandles);

      var firstBatchJobHandle = JobHandle.CombineDependencies(ballPursuerJobHandle, passerJobHandle, scorerJobHandle);

      this.jobHandle = JobHandle.CombineDependencies(firstBatchJobHandle, defenderJobHandle, disruptorJobHandle);

      JobHandle.ScheduleBatchedJobs();
    }

    public void ApplyJobResult() {
      if (!this.jobHandle.HasValue) {
        Debug.LogError("Apply job result called when no job handle exists");
        return;
      }

      this.jobHandle.Value.Complete();

      this.ProcessJobResults();
      this.ProcessJobResultsForDebug();

      this.jobHandle = null;
    }

    // TODO: probably move job result processing to schedulers
    private void ProcessJobResults() {
      for (int i = 0; i < this.curEvaluatingBuddioNum; ++i) {
        var buddio = this.gameContextHolder.Buddios.GetEntityWithId(this.buddiosArr[i].Id);

        var newRole = this.teamRoleAssignmentJobScheduler.GetJobResult(i).Role;

        var roleLogicResolutionJobResult = new RoleLogicResolutionJobResult(
          this.ballPursuerRoleLogicJobScheduler.GetJobResult(i),
          this.defenderRoleLogicJobScheduler.GetJobResult(i),
          this.disruptorRoleLogicJobScheduler.GetJobResult(i),
          this.passerRoleLogicJobScheduler.GetJobResult(i),
          this.scorerRoleLogicJobScheduler.GetJobResult(i));

        buddio.TryUpdateRoleWithJobResult(newRole, roleLogicResolutionJobResult);
      }
    }

    // TODO: probably move job result processing to schedulers
    private void ProcessJobResultsForDebug() {
      for (int i = 0; i < this.gameContextHolder.Balls.Entities.Count; ++i) {
        var positions = new List<Vector3>(BallFuturePosProjectionJobScheduler.FUTURE_PROJECTION_COUNT);
        for (int j = 0; j < BallFuturePosProjectionJobScheduler.FUTURE_PROJECTION_COUNT; ++j) {
          positions.Add(this.ballFuturePosProjectionJobScheduler.Results.GetPosForBall(i, j));
        }

        this.debugOutputter.DrawBallProjection(this.gameContextHolder.Balls.GetEntityWithId(this.ballsArr[i].Id), positions);
      }

      this.debugOutputter.DrawScorerBuddioDebugOutput(this.scorerRoleLogicJobScheduler.DebugResult);

      this.influenceMap.UpdateValues(this.influenceMapCalculationJobScheduler.GetJobResult().mapGrid);
      this.occupiedMap.UpdateValues(this.occupiedMapCalculationJobScheduler.GetJobResult().mapGrid);
    }

    public void WaitForCompleteAndDiscard() {
      this.jobHandle?.Complete();
    }

    private void PrepareJobs() {
      var buddios = this.gameContextHolder.Buddios.Entities;
      var balls = this.gameContextHolder.Balls.Entities;

      this.curEvaluatingBuddioNum = buddios.Count;

      this.PrepareJobInputs(buddios, balls);

      this.ballFuturePosProjectionJobScheduler.Setup(balls.Count, this.ballsArr);
      this.influenceMapCalculationJobScheduler.Setup(buddios.Count, this.buddiosArr);
      this.occupiedMapCalculationJobScheduler.Setup(buddios.Count, this.buddiosArr);
      this.teamRoleAssignmentJobScheduler.Setup(buddios.Count, this.buddiosArr, balls.Count, this.ballsArr);

      this.ballPursuerRoleLogicJobScheduler.Setup(buddios.Count, this.buddiosArr, balls.Count, this.ballsArr, this.teamRoleAssignmentJobScheduler.Results);
      this.defenderRoleLogicJobScheduler.Setup(buddios.Count, this.buddiosArr, balls.Count, this.ballsArr, this.teamRoleAssignmentJobScheduler.Results);
      this.disruptorRoleLogicJobScheduler.Setup(buddios.Count, this.buddiosArr, balls.Count, this.ballsArr, this.teamRoleAssignmentJobScheduler.Results);
      this.passerRoleLogicJobScheduler.Setup(buddios.Count, this.buddiosArr, balls.Count, this.ballsArr, this.teamRoleAssignmentJobScheduler.Results);
      this.scorerRoleLogicJobScheduler.Setup(buddios.Count, this.buddiosArr, balls.Count, this.ballsArr, this.teamRoleAssignmentJobScheduler.Results, this.occupiedMapCalculationJobScheduler.Result);
    }

    private void PrepareJobInputs(List<IGameBuddioFacade> buddios, List<IBallFacade> balls) {
      for (int i = 0; i < buddios.Count; ++i) {
        this.buddiosArr[i] = this.ConvertBuddioToBuddioData(buddios[i]);
      }

      for (int i = 0; i < balls.Count; ++i) {
        this.ballsArr[i] = this.ConvertBallToBuddioData(balls[i]);
      }
    }

    private GameBuddioData ConvertBuddioToBuddioData(IGameBuddioFacade buddioFacade) {
      return new GameBuddioData(buddioFacade.Id, buddioFacade.Buddio.Side, buddioFacade.Buddio.Pos, buddioFacade.Buddio.FacingDir, buddioFacade.Buddio.Stats, buddioFacade.CatchAreaRadius, buddioFacade.Buddio.Acting, buddioFacade.Buddio.HasBall, buddioFacade.Buddio.CanBeAttacked, buddioFacade.Buddio.ThrowPivotPos);
    }

    private GameBallData ConvertBallToBuddioData(IBallFacade ballFacade) {
      return new GameBallData(ballFacade.Id, ballFacade.GameBall.Caught, ballFacade.GameBall.Pos, ballFacade.GameBall.Vel, ballFacade.GameBall.Drag, ballFacade.GameBall.Mass);
    }
  }
}