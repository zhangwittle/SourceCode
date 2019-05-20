using NLog;

namespace BehaviorTree
{
    public class ConditionAimCompleted : ICondition
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private float _degle;

        public ConditionAimCompleted(float degle)
        {
            _degle = degle;
        }

        public bool Evaluate(InputParam inputParam)
        {
            float errorDegle = inputParam.GetAimingErrorDegle();

            //if (inputParam.killer.ID == 2 && errorDegle > _degle)
            //    logger.Error("playerID:{0}, errorDegle:{1}.", inputParam.killer.ID, errorDegle);

            return errorDegle < _degle;
        }
    }
}