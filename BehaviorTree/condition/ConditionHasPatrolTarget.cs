using Framework.Utils;
using NLog;

namespace BehaviorTree
{
    public class ConditionHasPatrolTarget : ICondition
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public bool Evaluate(InputParam inputParam)
        {
            return inputParam.hasMoveTarget && inputParam.moveTargetAIWayPointID != 0;
        }
    }
}