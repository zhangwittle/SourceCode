using BattleServer.Common;
using BattleServer.World;
using Framework.Utils;
using NLog;
using System.Collections.Generic;
using System.Numerics;
using static Framework.Utils.CFUtils;

namespace BehaviorTree
{
    public class AttackDecisionNode : BaseParallelNode, IWeightNode
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        float focusHaveMostSilverBadgeAttackTargetWeight = 10;
        public int GetWeight(InputParam inputParam)
        {
            int resultWeight = 0;
            if (inputParam.focusHaveMostSilverBadgeAttackTarget)
            {
                return Mathf.Ceil(focusHaveMostSilverBadgeAttackTargetWeight * WeightRandomInfo.FLOAT2INT_WEIGHT).ToInt();
            }
            if (!inputParam.HasEnemyInRadar(3 * CFConstant.PHYSICAL_FRAMES_RATE))
            {
                return resultWeight;
            }
            bool hasEnemyInDistance = false;
            List<Player> playerList = inputParam.GetEnemyInRadar(3 * CFConstant.PHYSICAL_FRAMES_RATE);
            for (int i = 0; i < playerList.Count; i++)
            {
                float distance = Vector3.Distance(inputParam.robotPlayer.Position, playerList[i].Position);
                //if (distance < 130)
                {
                    hasEnemyInDistance = true;
                    break;
                }
            }
            if (!hasEnemyInDistance)
            {
                return resultWeight;
            }
            int battleTeamType = inputParam.robotPlayer.battleTeamType;
            Vector3 playerPosition = inputParam.robotPlayer.Position;
            float killIncomeValue = inputParam.world.influenceMap.GetKillIncome(playerPosition, battleTeamType);
            float floatWeight = killIncomeValue;
            if (BATTLE_TYPE.isTeamBattleType(inputParam.world.statisticObject.battleType))
            {
                float safetyValue = inputParam.world.influenceMap.GetSafetyValue(playerPosition, battleTeamType);
                float threatValue = inputParam.world.influenceMap.GetThreatValue(playerPosition, battleTeamType);
                floatWeight += safetyValue - threatValue;
            }
            int weight = Mathf.Ceil(floatWeight * WeightRandomInfo.FLOAT2INT_WEIGHT).ToInt();
            if (resultWeight < weight)
            {
                resultWeight = weight;
            }
            //logger.Error("resultWeight:{0}.", resultWeight);
            return resultWeight;
        }
    }
}