using Framework.Utils;
using NLog;

namespace BehaviorTree
{
    public class ConditionResourceTargetVaild : ICondition
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public bool Evaluate(InputParam inputParam)
        {
            return inputParam.hasResourceTarget && inputParam.IsResourceTargetVaild() && !inputParam.IsInResourceTarget();
        }
    }
}