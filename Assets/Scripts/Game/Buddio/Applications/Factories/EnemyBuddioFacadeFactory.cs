using BuddioCore.Domain;
using Game.Applications;
using Game.Domain;
using UnityEngine;
using Zenject;

namespace Game.Buddio.Applications.Factories {
  public class EnemyBuddioFacadeFactory : BaseBuddioFacadeFactory {
    public EnemyBuddioFacadeFactory(GameContext gameContext) : base(Side.Enemy, gameContext, GameEntityIds.ENEMY_BUDDIO_START_ID) {
    }
  }
}