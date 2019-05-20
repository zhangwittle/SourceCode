using BehaviorTree;
using NLog;

namespace BehaviorTree
{
    public class FireActionNode : AbstractBaseActionNode
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private float _randomDegleMax = 2;

        public FireActionNode(float randomDegleMax)
        {
            _randomDegleMax = randomDegleMax;
        }

        //private static int robotPlayerID = 0;
        public override RunStatus Enter(InputParam inputParam)
        {
            //if (inputParam.robotPlayer.ID == robotPlayerID)
            //{
            //    logger.Error("playerID:{0}, curTankModelID:{1}, hasAttackTarget:{2}, GetAimingErrorDegle:{3}, CanFire:{4}, HasAimObstacle:{5}.", 
            //        inputParam.robotPlayer.ID, inputParam.robotPlayer.tankObject.curTankInfo.tableTank.ID, inputParam.hasAttackTarget, inputParam.GetAimingErrorDegle(), 
            //        inputParam.CanFire(), inputParam.HasAimObstacle());
            //}

            bool checkOk = EvaluatePrecondition(inputParam);
            if (!checkOk)
                return ChangeAndGetStatus(RunStatus.Failure);

            EnterAction(inputParam);

            return ChangeAndGetStatus(RunStatus.Running);
        }

        protected override void TickAction(InputParam inputParam, ref OutputParam outputParam)
        {
            if (!inputParam.HasAttackTag(AttackTag.AT_Attack))
            {
                //if (inputParam.robotPlayer.ID == 1002)
                //    logger.Error("playerID:{0}, attackTag:{1}. not attack.", inputParam.robotPlayer.ID, inputParam.attackTag);
                return;
            }

            if (inputParam.attackDeldayComplete())
            {
                //if (robotPlayerID == 0)
                //    robotPlayerID = inputParam.robotPlayer.ID;
                //if (inputParam.robotPlayer.ID == robotPlayerID)
                //{
                //    logger.Error("playerID:{0}, curTankModelID:{1}, attackTag:{2}. attack.",
                //           inputParam.robotPlayer.ID, inputParam.robotPlayer.tankObject.curTankInfo.tableTank.ID, inputParam.attackTag);
                //}
                if (inputParam.haveFireCount())
                {
                    inputParam.SetNewRandomAngele(_randomDegleMax);
                    outputParam.AddCommand(new FireCommand());
                    inputParam.accCurFireCount();
                }
                else
                {
                    inputParam.ResetAttackDelay();
                }
            }
            else
            {
                inputParam.accCurAttackDelay();
            }
        }
    }
}