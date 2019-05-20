using BehaviorTree;
using System.Numerics;

namespace BehaviorTree
{
    public class Move2SliverBadgePropPositionDecisionNode : AbstractBaseActionNode
    {
        protected override void EnterAction(InputParam inputParam)
        {
            inputParam.FindSliverBadgePropPostion();
        }

        protected override void TickAction(InputParam inputParam, ref OutputParam outputParam)
        {
            inputParam.MoveStep2PropPostion();
        }

        protected override void LeaveAction(InputParam inputParam)
        {
            inputParam.ClearPropTarget();
        }
    }
}