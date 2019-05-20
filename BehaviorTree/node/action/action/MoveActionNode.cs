using Framework.Utils;
using IronForce2;
using NLog;
using System.Numerics;

namespace BehaviorTree
{
    public class MoveActionNode : AbstractBaseActionNode
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        protected override void TickAction(InputParam inputParam, ref OutputParam outputParam)
        {
            if (!inputParam.HasAttackTag(AttackTag.AT_Move) && !inputParam.HasAttackTag(AttackTag.AT_Turn))
            {
                //if (inputParam.robotPlayer.ID == InputParam.testPlayerID)
                //    logger.Error("No AT_Move. playerID:{0}, attackTag:{1}. not move.", inputParam.robotPlayer.ID, inputParam.attackTag);
                return;
            }
            Vector3 bodyPos;
            Vector3 bodyDir;
            Vector3 bodyRightDir;
            inputParam.GetBodyPosDir(out bodyPos, out bodyDir, out bodyRightDir);
            float realSpeed = inputParam.robotPlayer.physicalObject.CurForwardSpeed();
            float maxSpeed = inputParam.robotPlayer.numericalObject.GetTankProperty(TankProperty.forwardMaxSpeed);
            float maxTurnSpeed = inputParam.robotPlayer.numericalObject.turnForwardMaxSpeed;
            bool needSlowMove = inputParam.robot.robotNavPath.needSlowMove;
            bool hasObstruction = inputParam.hasObstruction;
            MoveCommand moveCommand = new MoveCommand(inputParam.curMoveTargetPos, bodyPos, bodyDir, bodyRightDir, realSpeed, maxSpeed, maxTurnSpeed, needSlowMove, hasObstruction, 
                inputParam.HasAttackTag(AttackTag.AT_Move));
            outputParam.AddCommand(moveCommand);
            //if (inputParam.robotPlayer.ID == InputParam.testPlayerID)
            //    logger.Error("AddCommand. playerID:{0}, attackTag:{1}. not move.", inputParam.robotPlayer.ID, inputParam.attackTag);
            inputParam.ClearCurMoveTarget();

            //if (inputParam.killer.ID == 2)
            //{
            //    Vector2 targetDirNormalize = (inputParam.curMoveTargetPos.toVector2() - bodyPos.toVector2()).Normalize();
            //    Vector2 bodyDirNormalize = bodyDir.toVector2().Normalize();
            //    Vector2 bodyRightDirNormalize = bodyRightDir.toVector2().Normalize();

            //    float offsetValueY = Vector2.Dot(bodyDirNormalize, targetDirNormalize);//Y轴的偏移值，判断前后（点乘只能判断前后）
            //    float offsetValueX = Vector2.Dot(bodyRightDirNormalize, targetDirNormalize);

            //    if (offsetValueY < 0.3)
            //    {
            //        float finalMaxSpeed;
            //        if (offsetValueX == 0)
            //            finalMaxSpeed = maxSpeed;
            //        else
            //            finalMaxSpeed = maxTurnSpeed;

            //        float speedPercent = realSpeed / maxSpeed;

            //        if (speedPercent > 0.5)
            //            offsetValueY = -1;
            //        else if (speedPercent < -0.5)
            //            offsetValueY = 1;
            //        else
            //            offsetValueY = 0;

            //        if (offsetValueX > 0)
            //            offsetValueX = 1;
            //        else if (offsetValueX < 0)
            //            offsetValueX = -1;
            //    }

            //    if (offsetValueX < 0.1 && offsetValueX > -0.1)
            //        offsetValueX = 0;

            //    logger.Error("playerID:{0}, curPos:{1}, curMoveTargetPos:({2}, {3}, {4}), bodyDir:({5}, {6}, {7}), moveCommand:({8}, {9}).",
            //        inputParam.killer.ID, inputParam.killer.Position, inputParam.curMoveTargetPos.X, inputParam.curMoveTargetPos.Y, inputParam.curMoveTargetPos.Z,
            //        bodyDir.X, bodyDir.Y, bodyDir.Z, offsetValueX, offsetValueY);
            //}
        }
    }
}