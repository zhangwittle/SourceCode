using BehaviorTree;
using System.Numerics;

namespace BehaviorTree
{
    public class Move2ResourcePositionDecisionNode : AbstractBaseActionNode
    {
        protected override void EnterAction(InputParam inputParam)
        {
            inputParam.FindRandomResourcePostion();
        }

        protected override void TickAction(InputParam inputParam, ref OutputParam outputParam)
        {
            inputParam.MoveStep2ResourcePostion();
        }

        protected override void LeaveAction(InputParam inputParam)
        {
            inputParam.ClearResourceTarget();
        }
    }
}