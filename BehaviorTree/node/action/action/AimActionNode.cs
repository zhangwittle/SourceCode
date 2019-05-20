using BattleServer.World;
using BattleServer.World.AIUseBehaviorTree;
using BattleServer.World.PhysicsRef;
using BehaviorTree;
using Framework.Utils;
using System;
using System.Numerics;

namespace BehaviorTree
{
    public class AimActionNode : AbstractBaseActionNode
    {
        protected override void TickAction(InputParam inputParam, ref OutputParam outputParam)
        {
            float angle = 0;
            if (inputParam.HasHealTarget())
                angle = inputParam.GetHealDegle();
            else if (inputParam.hasAttackTarget)
                angle = inputParam.GetAimDegle();
            else if (inputParam.hasDetectTarget)
                angle = inputParam.GetDetectDegle();
            else
                angle = inputParam.GetBodyDegle();
            angle += inputParam.randomAngle;
            if (angle > PhysicsUtil.MAX_DEGLE)
                angle = PhysicsUtil.MAX_DEGLE;

            outputParam.AddCommand(new TurretAngleDegCommand(angle));

            //logger.Error("targetAngle:{0}, curAngle:{1}.", 
            //    angle, RayCastManager.GetTargetAngle(inputParam.robotPlayer.physicalObject.TurretDirection.toVector2().Normalize(), Vector2.UnitY));
        }
    }
}