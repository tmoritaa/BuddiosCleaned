using UnityEngine;

namespace Game.BuddioAi.Domain.EntityData {
  public readonly struct GameBallData {
    public readonly int Id;
    public readonly bool Caught;
    public readonly Vector3 Pos;
    public readonly Vector3 Vel;
    public readonly float Drag;
    public readonly float Mass;

    public GameBallData(int id, bool caught, Vector3 pos, Vector3 vel, float drag, float mass) {
      this.Id = id;
      this.Caught = caught;
      this.Pos = pos;
      this.Vel = vel;
      this.Drag = drag;
      this.Mass = mass;
    }
  }
}