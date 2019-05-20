using BattleServer.World.AIUseBehaviorTree;
using BehaviorTree;

namespace BehaviorTree
{
    public class ConditionReachAIWayPoint : ICondition
    {
        public bool Evaluate(InputParam inputParam)
        {
            return inputParam.ReachAIWayPoint();
        }
    }
}