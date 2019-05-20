using NLog;
using System.Collections.Generic;

namespace BehaviorTree
{
    public class BaseNode : INode
    {
        protected static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        protected string _name = "";
        protected RunStatus _status = RunStatus.Succeed;
        protected INode _parentNode;
        protected ICollection<ICondition> _preconditionList = new HashSet<ICondition>();
        protected int _tickInterval = 0;
        protected int _tickWaitFrame = 0;

        public string name { get { return _name; } set { _name = value; } }
        public RunStatus status { get { return _status; } set { _status = value; } }
        public INode parentNode { get { return _parentNode; } set { _parentNode = value; } }
        public ICollection<ICondition> preconditionList { get { return _preconditionList; } }
        public int tickInterval { get => _tickInterval; set => _tickInterval = value; }
        public int tickWaitFrame { get => _tickWaitFrame; }

        protected RunStatus ChangeAndGetStatus(RunStatus statusValue) { _status = statusValue; return statusValue; }
        public bool IsCompleted(RunStatus statusValue) { return statusValue == RunStatus.Succeed || statusValue == RunStatus.Failure; }

        protected void AddCondition(ICondition condition, ICollection<ICondition> conditionList) { conditionList.Add(condition); }
        protected void RemoveCondition(ICondition condition, ICollection<ICondition> conditionList) { conditionList.Remove(condition); }
        protected bool HasCondition(ICondition condition, ICollection<ICondition> conditionList) { return conditionList.Contains(condition); }

        protected virtual bool Evaluate(InputParam inputParam, ICollection<ICondition> conditionList)
        {
            foreach (ICondition condition in conditionList)
            {
                if (!condition.Evaluate(inputParam))
                    return false;
            }
            return true;
        }

        public void AddPrecondition(ICondition precondition) { AddCondition(precondition, _preconditionList); }
        public void RemovePrecondition(ICondition precondition) { RemoveCondition(precondition, _preconditionList); }
        public bool HasPrecondition(ICondition precondition) { return HasCondition(precondition, _preconditionList); }
        public virtual bool EvaluatePrecondition(InputParam inputParam) { return Evaluate(inputParam, _preconditionList); }

        public virtual RunStatus Enter(InputParam inputParam)
        {
            bool checkOk = EvaluatePrecondition(inputParam);
            if (!checkOk)
                return ChangeAndGetStatus(RunStatus.Failure);

            return ChangeAndGetStatus(RunStatus.Running);
        }

        public virtual RunStatus Tick(InputParam inputParam, ref OutputParam outputParam)
        {
            if (tickWaitFrame < tickInterval)
            {
                _tickWaitFrame++;
                return _status;
            }

            _tickWaitFrame = 0;
            return _status;
        }

        public virtual RunStatus Leave(InputParam inputParam)
        {
            return ChangeAndGetStatus(RunStatus.Failure);
        }
    }
}