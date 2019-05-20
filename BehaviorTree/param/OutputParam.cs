using BehaviorTree;
using System.Collections.Generic;

namespace BehaviorTree
{
    public class OutputParam
    {
        protected List<ICommand> _commandList = new List<ICommand>();

        public List<ICommand> commandList { get { return _commandList; } }

        public void AddCommand(ICommand command) { _commandList.Add(command); }
        public void RemoveCommand(ICommand command) { _commandList.Remove(command); }
        public bool HasCommand(ICommand command) { return _commandList.Contains(command); }
        public void ClearCommand() { _commandList.Clear(); }

        public virtual void Reset() { ClearCommand(); }
    }
}