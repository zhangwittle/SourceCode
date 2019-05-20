using BattleServer.World.AIUseBehaviorTree;
using BehaviorTree;

namespace BehaviorTree
{
    public class ConditionCanFire : ICondition
    {
        public bool Evaluate(InputParam inputParam)
        {
            return inputParam.CanFire();
        }
    }
}