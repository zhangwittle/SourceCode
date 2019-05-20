using BehaviorTree;
using NLog;
using System.Numerics;

namespace BehaviorTree
{
    public class ChooseWayPointDecisionNode : AbstractBaseActionNode
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private bool _findStartTarget = false;

        public ChooseWayPointDecisionNode(bool findStartTarget)
        {
            _findStartTarget = findStartTarget;
        }

        protected override void EnterAction(InputParam inputParam)
        {
            inputParam.FindPatrolTarget(_findStartTarget);

            //if (inputParam.killer.ID == 2)
            //    logger.Error("FindPatrolTarget. playerID:{0}.", inputParam.robot.killer.ID);
        }

        protected override void TickAction(InputParam inputParam, ref OutputParam outputParam)
        {
            inputParam.MoveStepMoveTarget();

            //if (inputParam.killer.ID == 2)
            //    logger.Error("MoveStepMoveTarget. playerID:{0}.", inputParam.robot.killer.ID);
        }

        protected override void LeaveAction(InputParam inputParam)
        {
            inputParam.ClearStartTarget();
            inputParam.ClearMoveTarget();
        }
    }
}