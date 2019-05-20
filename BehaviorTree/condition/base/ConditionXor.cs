namespace BehaviorTree
{
    public class ConditionXor : ICondition
    {
        private ICondition _condition1 = null;
        private ICondition _condition2 = null;

        public ConditionXor(ICondition condition1, ICondition condition2)
        {
            this._condition1 = condition1;
            this._condition2 = condition2;
        }

        public bool Evaluate(InputParam inputParam)
        {
            return _condition1.Evaluate(inputParam) ^ _condition2.Evaluate(inputParam);
        }
    }
}