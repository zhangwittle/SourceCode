using System.Collections.Generic;

namespace BehaviorTree
{
    public abstract class AbstractBaseActionNode : BaseNode
    {
        private ICollection<ICondition> _succeeConditionList = new HashSet<ICondition>();

        public void AddSucceeCondition(ICondition succeeCondition) { AddCondition(succeeCondition, _succeeConditionList); }
        public void RemoveSucceeCondition(ICondition succeeCondition) { RemoveCondition(succeeCondition, _succeeConditionList); }
        public bool HasSucceeCondition(ICondition succeeCondition) { return HasCondition(succeeCondition, _succeeConditionList); }
        public bool EvaluateSucceeCondition(InputParam inputParam)
        {
            if (_succeeConditionList.Count == 0)
                return false;
            return Evaluate(inputParam, _succeeConditionList);
        }

        public override RunStatus Enter(InputParam inputParam)
        {
            bool checkOk = EvaluatePrecondition(inputParam);
            if (!checkOk)
                return ChangeAndGetStatus(RunStatus.Failure);

            EnterAction(inputParam);

            return ChangeAndGetStatus(RunStatus.Running);
        }

        public override RunStatus Tick(InputParam inputParam, ref OutputParam outputParam)
        {
            bool checkOk = EvaluatePrecondition(inputParam);
            if (!checkOk)
                return ChangeAndGetStatus(RunStatus.Failure);

            TickAction(inputParam, ref outputParam);
            if (EvaluateSucceeCondition(inputParam))
                return ChangeAndGetStatus(RunStatus.Succeed);

            return ChangeAndGetStatus(RunStatus.Running);
        }

        public override RunStatus Leave(InputParam inputParam)
        {
            LeaveAction(inputParam);

            return ChangeAndGetStatus(RunStatus.Failure);
        }

        protected virtual void EnterAction(InputParam inputParam)
        {

        }

        protected virtual void TickAction(InputParam inputParam, ref OutputParam outputParam)
        {

        }

        protected virtual void LeaveAction(InputParam inputParam)
        {

        }
    }
}