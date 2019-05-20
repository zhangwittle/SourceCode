using BehaviorTree;
using System.Numerics;

namespace BehaviorTree
{
    public class FindDectectTargetDecisionNode : AbstractBaseActionNode
    {
        protected override void EnterAction(InputParam inputParam)
        {
            inputParam.FindDetectTarget();
        }

        protected override void LeaveAction(InputParam inputParam)
        {
            inputParam.ClearDetectTarget();
        }
    }
}