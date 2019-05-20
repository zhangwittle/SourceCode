using BattleServer.World.AIUseBehaviorTree;
using BehaviorTree;

namespace BehaviorTree
{
    public class ConditionDodgeDistance : ICondition
    {
        private float _distance;

        public ConditionDodgeDistance(float distance)
        {
            _distance = distance;
        }

        public bool Evaluate(InputParam inputParam)
        {
            return inputParam.GetDodgeTargetDistance() < _distance;
        }
    }
}