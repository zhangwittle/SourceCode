using NLog;
using System.Collections.Generic;

namespace BehaviorTree
{
    public class BaseParallelNode : AbstractBaseCompositeNode
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private ICollection<ICondition> _succeeConditionList = new HashSet<ICondition>();
        private ICollection<ICondition> _failConditionList = new HashSet<ICondition>();

        public void AddSucceeCondition(ICondition succeeCondition) { AddCondition(succeeCondition, _succeeConditionList); }
        public void RemoveSucceeCondition(ICondition succeeCondition) { RemoveCondition(succeeCondition, _succeeConditionList); }
        public bool HasSucceeCondition(ICondition succeeCondition) { return HasCondition(succeeCondition, _succeeConditionList); }
        public bool EvaluateSucceeCondition(InputParam inputParam)
        {
            if (_succeeConditionList.Count == 0)
                return false;
            return Evaluate(inputParam, _succeeConditionList);
        }

        public void AddFailCondition(ICondition failCondition) { AddCondition(failCondition, _failConditionList); }
        public void RemoveFailCondition(ICondition failCondition) { RemoveCondition(failCondition, _failConditionList); }
        public bool HasFailCondition(ICondition failCondition) { return HasCondition(failCondition, _failConditionList); }
        public bool EvaluateFailCondition(InputParam inputParam)
        {
            if (_failConditionList.Count == 0)
                return false;
            return Evaluate(inputParam, _failConditionList);
        }

        public override RunStatus Enter(InputParam inputParam)
        {
            bool checkOk = EvaluatePrecondition(inputParam);
            if (!checkOk)
                return ChangeAndGetStatus(RunStatus.Failure);

            return ChangeAndGetStatus(RunStatus.Running);
        }

        public override RunStatus Leave(InputParam inputParam)
        {
            foreach (INode runningNode in childList)
                runningNode.Leave(inputParam);

            return ChangeAndGetStatus(RunStatus.Failure);
        }

        public override RunStatus Tick(InputParam inputParam, ref OutputParam outputParam)
        {
            //if (inputParam.killer.ID == 2)
            //    logger.Error("{0} tick.", name);

            if (tickWaitFrame < tickInterval)
            {
                _tickWaitFrame++;
                return _status;
            }

            _tickWaitFrame = 0;

            if (IsCompleted(_status))
                return _status;

            bool checkOk = EvaluatePrecondition(inputParam);
            if (!checkOk)
            {
                foreach (INode childNode in childList)
                {
                    if (childNode.status == RunStatus.Running)
                        childNode.Leave(inputParam);
                }
                return ChangeAndGetStatus(RunStatus.Failure);
            }

            foreach (INode childNode in childList)
            {
                if (IsCompleted(childNode.status))
                    childNode.Enter(inputParam);
                if (!IsCompleted(childNode.status))
                    childNode.Tick(inputParam, ref outputParam);
            }

            if (EvaluateSucceeCondition(inputParam))
            {
                foreach (INode childNode in childList)
                    childNode.Leave(inputParam);
                return ChangeAndGetStatus(RunStatus.Succeed);
            }

            if (EvaluateFailCondition(inputParam))
            {
                foreach (INode childNode in childList)
                    childNode.Leave(inputParam);
                return ChangeAndGetStatus(RunStatus.Failure);
            }

            return ChangeAndGetStatus(RunStatus.Running);
        }
    }
}