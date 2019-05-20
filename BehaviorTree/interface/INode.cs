using System.Collections.Generic;

namespace BehaviorTree
{
    public interface INode
    {
        string name { get; set; }
        RunStatus status { get; }
        INode parentNode { get; set; }
        int tickInterval { get; set; }
        int tickWaitFrame { get; }

        ICollection<ICondition> preconditionList { get; }

        bool IsCompleted(RunStatus statusValue);

        void AddPrecondition(ICondition precondition);
        void RemovePrecondition(ICondition precondition);
        bool HasPrecondition(ICondition precondition);

        bool EvaluatePrecondition(InputParam inputParam);

        RunStatus Enter(InputParam inputParam);
        RunStatus Tick(InputParam inputParam, ref OutputParam outputParam);
        RunStatus Leave(InputParam inputParam);
    }
}