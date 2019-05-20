using Framework.Utils;
using NLog;

namespace BehaviorTree
{
    public class ConditionDetectTargetValid : ICondition
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public bool Evaluate(InputParam inputParam)
        {
            return inputParam.IsDetectTargetValid();
        }
    }
}