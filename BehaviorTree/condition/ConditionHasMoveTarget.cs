using Framework.Utils;
using NLog;

namespace BehaviorTree
{
    public class ConditionHasMoveTarget : ICondition
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public bool Evaluate(InputParam inputParam)
        {
            return inputParam.hasMoveTarget;
        }
    }
}