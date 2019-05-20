using BattleServer.World.AIUseBehaviorTree;
using BehaviorTree;

namespace BehaviorTree
{
    public class ConditionHasEnemyInRadar : ICondition
    {
        private int _limitFrame;

        public ConditionHasEnemyInRadar(int limitFrame)
        {
            _limitFrame = limitFrame;
        }

        public bool Evaluate(InputParam inputParam)
        {
            return inputParam.HasEnemyInRadar(_limitFrame);
        }
    }
}