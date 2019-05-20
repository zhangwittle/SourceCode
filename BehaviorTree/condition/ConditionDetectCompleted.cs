using NLog;

namespace BehaviorTree
{
    public class ConditionDetectCompleted : ICondition
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private float _degle;

        public ConditionDetectCompleted(float degle)
        {
            _degle = degle;
        }

        public bool Evaluate(InputParam inputParam)
        {
            float errorDegle = inputParam.GetDetectErrorDegle();

            return errorDegle < _degle;
        }
    }
}