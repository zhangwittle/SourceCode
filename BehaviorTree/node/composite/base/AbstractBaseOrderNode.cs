using System.Collections.Generic;

namespace BehaviorTree
{
    public abstract class AbstractBaseOrderNode : AbstractBaseCompositeNode
    {
        protected int _runningNodeIndex = 0;

        public override RunStatus Enter(InputParam inputParam)
        {
            bool checkOk = EvaluatePrecondition(inputParam);
            if (!checkOk)
                return ChangeAndGetStatus(RunStatus.Failure);

            INode runningNode = EnterNextNode(inputParam);
            if (null == runningNode)
                return ChangeAndGetStatus(RunStatus.Failure);

            return ChangeAndGetStatus(RunStatus.Running);
        }

        public override RunStatus Leave(InputParam inputParam)
        {
            if (_runningNodeIndex < childList.Count)
            {
                INode runningNode = childList[_runningNodeIndex];
                runningNode.Leave(inputParam);
            }
            _runningNodeIndex = 0;
            return ChangeAndGetStatus(RunStatus.Succeed);
        }

        protected abstract INode EnterNextNode(InputParam inputParam);
    }
}