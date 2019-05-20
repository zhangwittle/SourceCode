using BattleServer.World.AIUseBehaviorTree;

namespace BehaviorTree
{
    public class TurretAngleDegCommand : AbstractCommand
    {
        private float _turretAngleDeg;

        public TurretAngleDegCommand(float turretAngleDeg)
        {
            _turretAngleDeg = turretAngleDeg;
        }

        public override void Execute(ref UserCmd userCmd)
        {
            userCmd.TargetTurretAngleDegInWorld = _turretAngleDeg;
        }
    }
}