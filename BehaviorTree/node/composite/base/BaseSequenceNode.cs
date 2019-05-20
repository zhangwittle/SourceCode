namespace BehaviorTree
{
    public class BaseSequenceNode : AbstractBaseOrderNode
    {
        public override RunStatus Tick(InputParam inputParam, ref OutputParam outputParam)
        {
            if (IsCompleted(_status))
                return _status;

            bool checkOk = EvaluatePrecondition(inputParam);
            INode runningNode = childList[_runningNodeIndex];
            if (!checkOk)
            {
                if (runningNode.status == RunStatus.Running)
                    runningNode.Leave(inputParam);
                return ChangeAndGetStatus(RunStatus.Failure);
            }

            if (IsCompleted(runningNode.status))
                return ChangeAndGetStatus(runningNode.status);

            RunStatus newRunningStatus = runningNode.Tick(inputParam, ref outputParam);
            switch (newRunningStatus)
            {
                case RunStatus.Running:
                    return ChangeAndGetStatus(RunStatus.Running);
                case RunStatus.Succeed:
                    runningNode.Leave(inputParam);
                    _runningNodeIndex++;

                    bool hasNextNode = HasNextNode();
                    if (!hasNextNode)
                        return ChangeAndGetStatus(RunStatus.Succeed);
                    INode nextNode = EnterNextNode(inputParam);
                    if (nextNode == null)
                        return ChangeAndGetStatus(RunStatus.Failure);
                    return ChangeAndGetStatus(RunStatus.Running);
                case RunStatus.Failure:
                default:
                    _runningNodeIndex = 0;
                    return ChangeAndGetStatus(RunStatus.Failure);
            }
        }

        protected override INode EnterNextNode(InputParam inputParam)
        {
            if (!HasNextNode())
                return null;

            INode runningNode = childList[_runningNodeIndex];
            if (runningNode.Enter(inputParam) == RunStatus.Running)
                return runningNode;

            return null;
        }

        private bool HasNextNode()
        {
            if (_runningNodeIndex < childList.Count)
                return true;
            return false;
        }
    }
}