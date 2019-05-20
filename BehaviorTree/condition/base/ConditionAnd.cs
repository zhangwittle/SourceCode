namespace BehaviorTree
{
    public class ConditionAnd : ICondition
    {
        private ICondition[] _conditionArray = null;

        public ConditionAnd(ICondition[] conditionArray)
        {
            _conditionArray = conditionArray;
        }

        public bool Evaluate(InputParam inputParam)
        {
            foreach (ICondition condition in _conditionArray)
            {
                if (!condition.Evaluate(inputParam))
                    return false;
            }
            return true;
        }
    }
}