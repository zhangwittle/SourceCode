using BattleServer.World.AIUseBehaviorTree;
using BehaviorTree;

namespace BehaviorTree
{
    public class ConditionIsTargetAlive : ICondition
    {
        public bool Evaluate(InputParam inputParam)
        {
            return inputParam.IsTargetAlive();
        }
    }
}