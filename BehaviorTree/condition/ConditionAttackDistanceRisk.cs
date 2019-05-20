using BattleServer.World.AIUseBehaviorTree;
using BehaviorTree;
using NLog;

namespace BehaviorTree
{
    public class ConditionAttackDistanceRisk : ICondition
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public bool Evaluate(InputParam inputParam)
        {
            float distance = inputParam.GetAttackTargetDistance();

            float threatValue = inputParam.world.influenceMap.GetThreatValue(inputParam.robotPlayer.Position, inputParam.robotPlayer.battleTeamType);
            float safetyValue = inputParam.world.influenceMap.GetSafetyValue(inputParam.robotPlayer.Position, inputParam.robotPlayer.battleTeamType);
            float distacneRatio = threatValue / safetyValue;

            //if (inputParam.killer.ID == 2)
            //    logger.Error("playerID:{0}, distacneRatio:{1}.", inputParam.killer.ID, distacneRatio);

            if (distacneRatio > 1)
                distacneRatio = 1;
            float targetDistance = 100 * distacneRatio;

            //if (inputParam.killer.ID == 2)
            //    logger.Error("playerID:{0}, distance:{1}, targetDistance:{2}.", inputParam.killer.ID, distance, targetDistance);

            return distance < targetDistance;
        }
    }
}