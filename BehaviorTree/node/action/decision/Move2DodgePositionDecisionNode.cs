using BehaviorTree;
using System.Numerics;

namespace BehaviorTree
{
    public class Move2DodgePositionDecisionNode : AbstractBaseActionNode
    {
        protected override void EnterAction(InputParam inputParam)
        {
            inputParam.FindRandomDodgePostion();
        }

        protected override void TickAction(InputParam inputParam, ref OutputParam outputParam)
        {
            inputParam.MoveStep2DodgePostion();
        }

        protected override void LeaveAction(InputParam inputParam)
        {
            inputParam.ClearDodgeMoveTarget();
        }
    }
}