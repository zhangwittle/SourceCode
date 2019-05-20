namespace BehaviorTree
{
    public class FireCommand : AbstractCommand
    {
        public override void Execute(ref UserCmd userCmd)
        {
            userCmd.fireButton = true;
        }
    }
}