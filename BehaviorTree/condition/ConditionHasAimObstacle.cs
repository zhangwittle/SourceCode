using BattleServer.World.AIUseBehaviorTree;
using BehaviorTree;
using NLog;

namespace BehaviorTree
{
    public class ConditionHasAimObstacle : ICondition
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public bool Evaluate(InputParam inputParam)
        {
            bool hasAimObstacle = inputParam.HasAimObstacle();

            //if (inputParam.killer.ID == 2 && hasAimObstacle)
            //    logger.Error("playerID:{0}, hasAimObstacle:{1}.", inputParam.killer.ID, hasAimObstacle);
            
            return hasAimObstacle;
        }
    }
}