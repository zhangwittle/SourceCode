using BattleServer.World.AIUseBehaviorTree;
using BehaviorTree;
using NLog;

namespace BehaviorTree
{
    public class ConditionAttackDistance : ICondition
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private float _distance;

        public ConditionAttackDistance(float distance)
        {
            _distance = distance;
        }

        public bool Evaluate(InputParam inputParam)
        {
            float distance = inputParam.GetAttackTargetDistance();
            return distance < _distance;
        }
    }
}