using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorTree
{
    public class FindHealTargetDecisionNode : AbstractBaseActionNode
    {
        public FindHealTargetDecisionNode()
        {
            
        }

        protected override void TickAction(InputParam inputParam, ref OutputParam outputParam)
        {
            inputParam.FindHealTarget();
        }
    }
}
