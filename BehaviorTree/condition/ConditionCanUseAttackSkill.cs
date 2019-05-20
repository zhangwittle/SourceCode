using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorTree
{
    public class ConditionCanUseAttackSkill : ICondition
    {
        public bool Evaluate(InputParam inputParam)
        {            
            return inputParam.CanUseAttackSkill();
        }
    }
}
