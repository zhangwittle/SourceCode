using Com.Coolfish.Ironforce2.Protocol.Msg;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorTree
{
    public class HealSkillActionNode : AbstractBaseActionNode
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private int skillDelayFrame = 15;
        private int curDelayFrame = 0;

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
            if (curDelayFrame >= skillDelayFrame)
            {                
                curDelayFrame = 0;
                int healSkill = inputParam.robotPlayer.SkillManager.HealSkillReady();
                int healTarget = inputParam.healTargetID;
                if (healSkill > 0 && healTarget > 0)
                {
                    BCxPlayerActiveSkillEvent msg = new BCxPlayerActiveSkillEvent();
                    msg.CasterId = inputParam.robotPlayer.ID;
                    msg.SkillId = healSkill;
                    msg.Angle = inputParam.robotPlayer.physicalObject.GetTurretAngleWorld();
                    msg.Targetlist.Add(healTarget);
                    inputParam.robotPlayer.SkillManager.ActiveSkill(msg);
                    inputParam.ClearHealSkill();
                }
                else
                {
                    inputParam.ClearHealSkill();
                }
                skillDelayFrame = 5;
            }
            else
                curDelayFrame++;
        }
    }
}
