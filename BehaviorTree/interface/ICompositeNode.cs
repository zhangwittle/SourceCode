using System.Collections.Generic;

namespace BehaviorTree
{
    public interface ICompositeNode : INode
    {
        List<INode> childList { get; }

        void AddNode(INode node);
        void RemoveNode(INode node);
        bool HasNode(INode node);
    }
}