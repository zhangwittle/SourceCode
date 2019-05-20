using NLog;

namespace BehaviorTree
{
    public class BaseSelectorNode : AbstractBaseOrderNode
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

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
                    return ChangeAndGetStatus(RunStatus.Succeed);
                case RunStatus.Failure:
                default:
                    runningNode.Leave(inputParam);
                    _runningNodeIndex++;

                    INode nextNode = EnterNextNode(inputParam);
                    if (nextNode == null)
                        return ChangeAndGetStatus(RunStatus.Failure);
                    return ChangeAndGetStatus(RunStatus.Running);
            }
        }

        protected override INode EnterNextNode(InputParam inputParam)
        {
            if (_runningNodeIndex >= childList.Count)
            {
                //if (inputParam.robotPlayer.ID == 2)
                //    logger.Error("name:{0}, no node.", name);
                _runningNodeIndex = 0;
                return null;
            }

            do
            {
                INode runningNode = childList[_runningNodeIndex];
                if (runningNode.Enter(inputParam) == RunStatus.Running)
                {
                    //if (inputParam.robotPlayer.ID == 2)
                    //    logger.Error("name:{0}, nodeName:{1}.", name, runningNode.name);
                    return runningNode;
                }

                _runningNodeIndex++;
            } while (_runningNodeIndex < childList.Count);

            _runningNodeIndex = 0;
            //if (inputParam.robotPlayer.ID == 2)
            //    logger.Error("name:{0}, no node.", name);
            return null;
        }
    }
}