namespace BehaviorTree
{
    public class ConditionTrue : ICondition
    {
        public bool Evaluate(InputParam inputParam)
        {
            return true;
        }
    }
}