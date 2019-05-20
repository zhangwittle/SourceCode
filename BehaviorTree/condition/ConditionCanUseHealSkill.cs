using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorTree
{
    public class ConditionCanUseHealSkill : ICondition
    {
        public bool Evaluate(InputParam inputParam)
        {
            int healSkill = inputParam.robotPlayer.SkillManager.HealSkillReady();
            int healTarget = inputParam.healTargetID;
            if (healSkill > 0 && healTarget > 0)
            {
                float errorDegle = inputParam.GetHealAimingErrorDegle();
                return errorDegle < 15;
            }
            return false;
        }
    }
}
