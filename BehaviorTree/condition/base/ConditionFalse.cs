namespace BehaviorTree
{
    public class ConditionFalse : ICondition
    {
        public bool Evaluate(InputParam inputParam)
        {
            return false;
        }
    }
}