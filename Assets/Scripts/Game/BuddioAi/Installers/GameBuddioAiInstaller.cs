using Game.BuddioAi.Applications;
using Game.BuddioAi.Domain.DataMaps;
using Game.BuddioAi.Jobs.BallFuturePosProjection.Domain;
using Game.BuddioAi.Jobs.InfluenceMapCalculation.Domain;
using Game.BuddioAi.Jobs.OccupiedMapCalculation.Domain;
using Game.BuddioAi.Jobs.RoleLogic.BallPursuerRoleLogic.Domain;
using Game.BuddioAi.Jobs.RoleLogic.DefenderRoleLogic.Domain;
using Game.BuddioAi.Jobs.RoleLogic.DisruptorRoleLogic.Domain;
using Game.BuddioAi.Jobs.RoleLogic.PasserRoleLogic.Domain;
using Game.BuddioAi.Jobs.RoleLogic.ScorerRoleLogic.Domain;
using Game.BuddioAi.Jobs.TeamRoleAssignment.Domain;
using Zenject;

namespace Game.BuddioAi.Installers {
  public class GameBuddioAiInstaller : Installer<GameBuddioAiInstaller> {
    public override void InstallBindings() {
      this.Container.BindInterfacesAndSelfTo<BallFuturePosProjectionJobScheduler>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<InfluenceMapCalculationJobScheduler>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<OccupiedMapCalculationJobScheduler>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<TeamRoleAssignmentJobScheduler>().AsSingle();

      this.Container.BindInterfacesAndSelfTo<BallPursuerRoleLogicJobScheduler>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<DefenderRoleLogicJobScheduler>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<DisruptorRoleLogicJobScheduler>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<PasserRoleLogicJobScheduler>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<ScorerRoleLogicJobScheduler>().AsSingle();

      this.Container.BindInterfacesAndSelfTo<BuddioAiScheduler>().AsSingle();

      this.Container.BindInterfacesAndSelfTo<InfluenceMap>().AsSingle();
      this.Container.BindInterfacesAndSelfTo<OccupiedMap>().AsSingle();
    }
  }
}