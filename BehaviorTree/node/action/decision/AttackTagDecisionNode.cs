using BehaviorTree;
using Framework.Utils;
using NLog;
using System.Numerics;

namespace BehaviorTree
{
    public class AttackTagDecisionNode : AbstractBaseActionNode
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private int _attackTag;

        public AttackTagDecisionNode(int attackTag)
        {
            _attackTag = attackTag;
        }

        protected override void TickAction(InputParam inputParam, ref OutputParam outputParam)
        {
            inputParam.attackTag = _attackTag;
            //if (inputParam.robotPlayer.ID == 1002)
            //    logger.Error("playerID:{0}, attackTag:{1}, parentName:{2}.", inputParam.robotPlayer.ID, inputParam.attackTag, _parentNode != null ? _parentNode.name : "");
        }

        protected override void LeaveAction(InputParam inputParam)
        {
            inputParam.attackTag = AttackTag.AT_None.ToInt();
            //if (inputParam.robotPlayer.ID == 1002)
            //    logger.Error("playerID:{0}, attackTag:{1}, parentName:{2}.", inputParam.robotPlayer.ID, inputParam.attackTag, _parentNode != null ? _parentNode.name : "");
        }
    }
}