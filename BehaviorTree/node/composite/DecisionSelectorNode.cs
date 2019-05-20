using BattleServer.Common;
using NLog;
using System.Collections.Generic;
using static Framework.Utils.CFUtils;

namespace BehaviorTree
{
    public class DecisionSelectorNode : BaseNode
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private const float vieMinRatio = 1.3f;
        private const int nodeDurationMin = (int) (0.3f * CFConstant.PHYSICAL_FRAMES_RATE);
        private int _curNodeStartFrame = 0;

        private List<IWeightNode> _weightChildList = new List<IWeightNode>();
        private INode _defaultChild = null;
        private INode _runningNode = null;

        public void AddWeightNode(IWeightNode node) { node.parentNode = this; _weightChildList.Add(node); }
        public void RemoveWeightNode(IWeightNode node) { _weightChildList.Remove(node); }
        public bool HasWeightNode(IWeightNode node) { return _weightChildList.Contains(node); }

        public void SetDefaultNode(INode node) { node.parentNode = this; _defaultChild = node; }

        public override bool EvaluatePrecondition(InputParam inputParam)
        {
            if (_weightChildList.Count == 0 && _defaultChild == null)
                return false;

            return base.EvaluatePrecondition(inputParam);
        }

        public override RunStatus Enter(InputParam inputParam)
        {
            bool checkOk = EvaluatePrecondition(inputParam);
            if (!checkOk)
                return ChangeAndGetStatus(RunStatus.Failure);

            return ChangeAndGetStatus(RunStatus.Running);
        }

        public override RunStatus Tick(InputParam inputParam, ref OutputParam outputParam)
        {
            if (inputParam.currentFrame - _curNodeStartFrame > nodeDurationMin)
            {
                INode nextNode = SelectNextNode(inputParam);
                if (null == nextNode)
                    return ChangeAndGetStatus(RunStatus.Failure);

                if (_runningNode != nextNode)
                {
                    if (_runningNode != null)
                        _runningNode.Leave(inputParam);
                    _runningNode = nextNode;

                    _runningNode.Enter(inputParam);

                    //if (inputParam.killer.ID == 2)
                    //    logger.Error("_runningNodeName:{0}.", _runningNode.name);

                    _curNodeStartFrame = inputParam.currentFrame;
                }
            }

            //if (inputParam.killer.ID == 2)
            //    logger.Error("_runningNodeName:{0}.", _runningNode.name);

            _runningNode.Tick(inputParam, ref outputParam);

            return RunStatus.Running;
        }

        public override RunStatus Leave(InputParam inputParam)
        {
            if (_runningNode != null)
            {
                _runningNode.Leave(inputParam);
                _runningNode = null;
            }

            return ChangeAndGetStatus(RunStatus.Succeed);
        }

        private INode SelectNextNode(InputParam inputParam)
        {
            int vieWeight = 0;

            List<WeightRandomInfo> weightRandomInfoList = new List<WeightRandomInfo>();
            for (int i = 0; i < _weightChildList.Count; i++)
            {
                int id = i + 1;
                int weight = _weightChildList[i].GetWeight(inputParam);

                if (weight <= 0)
                    continue;

                WeightRandomInfo weightRandomInfo = new WeightRandomInfo(id, weight);
                weightRandomInfoList.Add(weightRandomInfo);

                if (_runningNode == null || _runningNode.IsCompleted(_runningNode.status))
                    continue;

                if (_runningNode == _weightChildList[i])
                    vieWeight = weightRandomInfo.weight;
            }

            if (vieWeight != 0)
                vieWeight = (vieWeight.ToFloat() * vieMinRatio).ToInt();

            List<WeightRandomInfo> finalWeightRandomInfoList = new List<WeightRandomInfo>();
            foreach (WeightRandomInfo weightRandomInfo in weightRandomInfoList)
            {
                if (vieWeight != 0 && _runningNode != _weightChildList[weightRandomInfo.id - 1] && weightRandomInfo.weight <= vieWeight)
                {
                    //if (_runningNode is AttackDecisionNode && _weightChildList[weightRandomInfo.id - 1] is LootPropDecisionNode)
                    //{
                    //    logger.Error("nodeName:{0}, weight:{1}, viewWeight:{2}.", _weightChildList[weightRandomInfo.id - 1].name, weightRandomInfo.weight, vieWeight);
                    //}
                    continue;
                }

                finalWeightRandomInfoList.Add(weightRandomInfo);
                //if (inputParam.robotPlayer.ID == 2)
                //    logger.Error("nodeName:{0}, weight:{1}.", _weightChildList[weightRandomInfo.id - 1].name, weightRandomInfo.weight);
            }

            WeightRandomInfo selectWeightRandomInfo = WeightMax(finalWeightRandomInfoList);
            if (selectWeightRandomInfo != null)
            {
                //if (inputParam.robotPlayer.ID == 2)
                //    logger.Error("select nodeName:{0}.", _weightChildList[selectWeightRandomInfo.id - 1].name);
                return _weightChildList[selectWeightRandomInfo.id - 1];
            }

            //if (inputParam.robotPlayer.ID == 2)
            //    logger.Error("select nodeName:{0}.", _defaultChild.name);
            return _defaultChild;
        }
    }
}