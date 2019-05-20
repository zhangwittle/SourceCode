using System.Collections.Generic;

namespace BehaviorTree
{
    public abstract class AbstractBaseCompositeNode : BaseNode, ICompositeNode
    {
        protected List<INode> _childList = new List<INode>();

        public List<INode> childList { get { return _childList; } }

        public void AddNode(INode node) { node.parentNode = this; _childList.Add(node); }
        public void RemoveNode(INode node) { _childList.Remove(node); }
        public bool HasNode(INode node) { return _childList.Contains(node); }

        public override bool EvaluatePrecondition(InputParam inputParam)
        {
            if (_childList.Count == 0)
                return false;

            return base.EvaluatePrecondition(inputParam);
        }
    }
}