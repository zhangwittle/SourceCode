using Framework.Utils;
using NLog;

namespace BehaviorTree
{
    public class ConditionHasCurMoveTarget : ICondition
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public bool Evaluate(InputParam inputParam)
        {
            //if (inputParam.robotPlayer.ID == InputParam.testPlayerID)
            //    logger.Error("ConditionHasCurMoveTarget. playerID:{0}, hasCurMoveTarget:{1}.", inputParam.robotPlayer.ID, inputParam.hasCurMoveTarget);
            return inputParam.hasCurMoveTarget;
        }
    }
}