using NLog;

namespace BehaviorTree
{
    public class ConditionNot : ICondition
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private ICondition _condition = null;

        public ConditionNot(ICondition condition)
        {
            this._condition = condition;
        }

        public bool Evaluate(InputParam inputParam)
        {
            return !_condition.Evaluate(inputParam);
        }
    }
}