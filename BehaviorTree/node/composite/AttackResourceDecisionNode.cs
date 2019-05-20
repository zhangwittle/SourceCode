using BattleServer.Common;
using BattleServer.World;
using Framework.Utils;
using NLog;
using System.Collections.Generic;
using System.Numerics;
using static Framework.Utils.CFUtils;

namespace BehaviorTree
{
    public class AttackResourceDecisionNode : BaseParallelNode, IWeightNode
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public int GetWeight(InputParam inputParam)
        {
            int resultWeight = 0;

            float dominationIncome = inputParam.world.influenceMap.GetDominationIncome(inputParam.robotPlayer.Position, inputParam.robotPlayer.battleTeamType, 0.8f, false);
            resultWeight = Mathf.Ceil(dominationIncome * WeightRandomInfo.FLOAT2INT_WEIGHT).ToInt() * inputParam.world.playerController.playerCount / 2;

            //if (inputParam.robotPlayer.ID == 2)
            //    logger.Error("weight:{0}.", resultWeight);

            return resultWeight;
        }
    }
}