namespace BehaviorTree
{
    public class ConditionOr : ICondition
    {
        private ICondition[] _conditionArray = null;

        public ConditionOr(ICondition[] conditionArray)
        {
            _conditionArray = conditionArray;
        }

        public bool Evaluate(InputParam inputParam)
        {
            foreach (ICondition condition in _conditionArray)
            {
                if (condition.Evaluate(inputParam))
                    return true;
            }
            return false;
        }
    }
}