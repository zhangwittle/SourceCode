namespace BehaviorTree
{
    public interface ICondition
    {
        bool Evaluate(InputParam inputParam);
    }
}