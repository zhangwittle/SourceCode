using Framework.Utils;
using NLog;

namespace BehaviorTree
{
    public class ConditionInResourceTarget : ICondition
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public bool Evaluate(InputParam inputParam)
        {
            return inputParam.hasResourceTarget && inputParam.IsInResourceTarget();
        }
    }
}