using BattleServer.World.AIUseBehaviorTree;
using BehaviorTree;

namespace BehaviorTree
{
    public class ConditionIsTargetTankPowerSmaller : ICondition
    {
        public bool Evaluate(InputParam inputParam)
        {
            return inputParam.TargetTankPowerIsSmaller();
        }
    }
}