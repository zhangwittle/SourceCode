using BattleServer.World.AIUseBehaviorTree;
using BehaviorTree;

namespace BehaviorTree
{
    public abstract class AbstractCommand : ICommand
    {
        public abstract void Execute(ref UserCmd userCmd);
    }
}