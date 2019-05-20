using BattleServer.Common;
using BattleServer.World;
using Framework.Utils;
using NLog;
using System.Collections.Generic;
using System.Numerics;
using static Framework.Utils.CFUtils;

namespace BehaviorTree
{
    public class GuideAttackDecisionNode : BaseParallelNode
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public override RunStatus Leave(InputParam inputParam)
        {
            inputParam.ClearAttackTarget();
            return base.Leave(inputParam);
        }
    }
}