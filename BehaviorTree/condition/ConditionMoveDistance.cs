using BattleServer.World.AIUseBehaviorTree;
using BehaviorTree;

namespace BehaviorTree
{
    public class ConditionMoveDistance : ICondition
    {
        private float _distance;

        public ConditionMoveDistance(float distance)
        {
            _distance = distance;
        }

        public bool Evaluate(InputParam inputParam)
        {
            return inputParam.GetMoveTargetDistance() < _distance;
        }
    }
}