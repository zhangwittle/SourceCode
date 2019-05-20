using Framework.Utils;
using NLog;

namespace BehaviorTree
{
    public class ConditionHasDodgeTarget : ICondition
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public bool Evaluate(InputParam inputParam)
        {
            //if (inputParam.hasDodgeMoveTarget)
            //{
            //    if (inputParam.robotPlayer.ID == 2)
            //        logger.Error("hasDodgeMoveTarget.");
            //}
                
            return inputParam.hasDodgeMoveTarget;
        }
    }
}