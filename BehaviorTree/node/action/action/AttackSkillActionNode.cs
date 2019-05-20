using Com.Coolfish.Ironforce2.Protocol.Msg;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorTree
{
    public class AttackSkillActionNode : AbstractBaseActionNode
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private int skillDelayFrame = 15;
        private float _randomAngleMax = 2;
        private int curDelayFrame = 0;
        private Random _random = new Random();

        public AttackSkillActionNode(float randomAngleMax)
        {
            _randomAngleMax = randomAngleMax;
        }

        //private static int robotPlayerID = 0;
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
            if (!inputParam.HasAttackTag(AttackTag.AT_Attack))
            {
                //if (inputParam.robotPlayer.ID == 1002)
                //    logger.Error("playerID:{0}, attackTag:{1}. not attack.", inputParam.robotPlayer.ID, inputParam.attackTag);
                return;
            }
            
            if (curDelayFrame >= skillDelayFrame)
            {  
                inputParam.SetNewRandomAngele(_randomAngleMax);
                curDelayFrame = 0;
                if (inputParam.attackSkillID > 0)
                {                    
                    BCxPlayerActiveSkillEvent msg = new BCxPlayerActiveSkillEvent();
                    msg.CasterId = inputParam.robotPlayer.ID;
                    msg.SkillId = inputParam.attackSkillID;
                    msg.Angle = inputParam.robotPlayer.physicalObject.GetTurretAngleWorld();

                    if (msg.SkillId == 8 && inputParam.attackTargetID > 0) // for sky fire
                    {
                        msg.Targetlist.Add(inputParam.attackTargetID);
                    }
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
