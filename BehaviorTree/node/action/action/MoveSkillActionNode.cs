using Com.Coolfish.Ironforce2.Protocol.Msg;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorTree
{
    public class MoveSkillActionNode : AbstractBaseActionNode
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private int skillDelayFrame = 5;
        private float _randomAngleMax = 2;
        private int curDelayFrame = 0;
        private Random _random = new Random();

        public MoveSkillActionNode(float randomAngleMax)
        {
            _randomAngleMax = randomAngleMax;
        }

        public override RunStatus Enter(InputParam inputParam)
        {
            bool checkOk = EvaluatePrecondition(inputParam);
            if (!checkOk)
                return ChangeAndGetStatus(RunStatus.Failure);

            EnterAction(inputParam);

            return ChangeAndGetStatus(RunStatus.Running);
        }

        protected override void TickAction(InputParam inputParam, ref OutputParam outputParam)
        {
            if (!inputParam.HasAttackTag(AttackTag.AT_Move))
            {
                return;
            }
            
            if (curDelayFrame >= skillDelayFrame)
            {
                inputParam.SetNewRandomAngele(_randomAngleMax);
                curDelayFrame = 0;
                if (inputParam.moveSkillID > 0)
                {                 
                    BCxPlayerActiveSkillEvent msg = new BCxPlayerActiveSkillEvent();
                    msg.CasterId = inputParam.robotPlayer.ID;
                    msg.SkillId = inputParam.moveSkillID;
                    msg.Angle = inputParam.robotPlayer.physicalObject.GetTurretAngleWorld();
                    inputParam.robotPlayer.SkillManager.ActiveSkill(msg);
                    inputParam.ClearAttackSkill();
                }
                skillDelayFrame = 5 + _random.Next(15, 75);
            }
            else
                curDelayFrame++;
        }
    }
}
