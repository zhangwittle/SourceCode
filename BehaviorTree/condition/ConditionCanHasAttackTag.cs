using BattleServer.World.AIUseBehaviorTree;
using BehaviorTree;

namespace BehaviorTree
{
    public class ConditionCanHasAttackTag : ICondition
    {
        private AttackTag _attackTag;

        public ConditionCanHasAttackTag(AttackTag attackTag)
        {
            _attackTag = attackTag;
        }

        public bool Evaluate(InputParam inputParam)
        {
            return inputParam.CanHasAttackTag(_attackTag);
        }
    }
}