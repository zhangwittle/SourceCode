using BehaviorTree;
using System.Numerics;

namespace BehaviorTree
{
    public class FindAttackTargetDecisionNode : AbstractBaseActionNode
    {
        private int _limitFrame;
        private bool _findHaveMostSilverBadgeAttackTarget;
        private int _maxFocusRobotNum;

        public FindAttackTargetDecisionNode(int limitFrame, bool findHaveMostSilverBadgeAttackTarget, int maxFocusRobotNum)
        {
            _limitFrame = limitFrame;
            _findHaveMostSilverBadgeAttackTarget = findHaveMostSilverBadgeAttackTarget;
            _maxFocusRobotNum = maxFocusRobotNum;
        }

        protected override void TickAction(InputParam inputParam, ref OutputParam outputParam)
        {
            inputParam.FindAttackTarget(_limitFrame, _findHaveMostSilverBadgeAttackTarget, _maxFocusRobotNum);
        }
    }
}