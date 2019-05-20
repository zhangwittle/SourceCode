using BehaviorTree;
using System.Numerics;

namespace BehaviorTree
{
    public class Move2AttackTargetDecisionNode : AbstractBaseActionNode
    {
        protected override void TickAction(InputParam inputParam, ref OutputParam outputParam)
        {
            inputParam.MoveStepAttackTarget();
        }
    }
}