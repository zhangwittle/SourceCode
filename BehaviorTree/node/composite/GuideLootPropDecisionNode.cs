using BattleServer.Common;
using BattleServer.World;
using Framework.Utils;
using NLog;
using System.Collections.Generic;
using System.Numerics;
using static Framework.Utils.CFUtils;

namespace BehaviorTree
{
    public class GuideLootPropDecisionNode : BaseParallelNode, IWeightNode
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public int GetWeight(InputParam inputParam)
        {
            int resultWeight = 0;

            if (inputParam.world.scriptManager.GetSilverBadgePropCount() == 0)
                return resultWeight;

            resultWeight = WeightRandomInfo.FLOAT2INT_WEIGHT;

            return resultWeight;
        }
    }
}