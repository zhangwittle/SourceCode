namespace BehaviorTree
{
    public interface IWeightNode : INode
    {
        int GetWeight(InputParam inputParam);
    }
}