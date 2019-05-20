using BattleServer.Common;
using BattleServer.World;
using Framework.Utils;
using NLog;
using System.Collections.Generic;
using System.Numerics;
using static Framework.Utils.CFUtils;

namespace BehaviorTree
{
    public class LootPropDecisionNode : BaseParallelNode, IWeightNode
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private float _weightRatio = 1;

        public LootPropDecisionNode(float weightRatio)
        {
            _weightRatio = weightRatio;
        }

        public int GetWeight(InputParam inputParam)
        {
            int resultWeight = 0;

            float propIncomeValue = inputParam.world.influenceMap.GetPropIncome(inputParam.robotPlayer.Position, 0.8f);
            resultWeight = Mathf.Ceil(propIncomeValue * _weightRatio * WeightRandomInfo.FLOAT2INT_WEIGHT).ToInt();

            //if (inputParam.robotPlayer.ID == 2)
            //    logger.Error("weight:{0}.", resultWeight);

            return resultWeight;
        }
    }
}