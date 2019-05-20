using Framework.Utils;
using NLog;

namespace BehaviorTree
{
    public class ConditionHasPropTarget : ICondition
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public bool Evaluate(InputParam inputParam)
        {
            return inputParam.hasPropTarget && inputParam.IsPropTargetExist();
        }
    }
}