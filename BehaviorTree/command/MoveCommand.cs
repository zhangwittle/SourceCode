using BattleServer.World.PhysicsRef;
using Framework.Utils;
using NLog;
using System;
using System.Numerics;

namespace BehaviorTree
{
    public class MoveCommand : AbstractCommand
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private Vector3 _targetPosition;
        private Vector3 _bodyPos;
        private Vector3 _bodyDir;
        private Vector3 _bodyRightDir;
        private float _realSpeed;
        private float _maxSpeed;
        private float _maxTurnSpeed;
        private bool _needSlowMove;
        private bool _hasObstruction;
        private bool _move;

        public MoveCommand(Vector3 targetPosition, Vector3 bodyPos, Vector3 bodyDir, Vector3 bodyRightDir, float realSpeed, float maxSpeed, float maxTurnSpeed, bool needSlowMove, 
            bool hasObstruction, bool move)
        {
            _targetPosition = targetPosition;
            _bodyPos = bodyPos;
            _bodyDir = bodyDir;
            _bodyRightDir = bodyRightDir;
            _realSpeed = realSpeed;
            _maxSpeed = maxSpeed;
            _maxTurnSpeed = maxTurnSpeed;
            _needSlowMove = needSlowMove;
            _hasObstruction = hasObstruction;
            _move = move;
        }

        public override void Execute(ref UserCmd userCmd)
        {
            if (_move)
            {
                Move(ref userCmd);
            }
            else
            {
                Turn(ref userCmd);
            }
        }

        private void Move(ref UserCmd userCmd)
        {
            Vector2 targetDirNormalize = (_targetPosition.toVector2() - _bodyPos.toVector2()).Normalize();
            Vector2 bodyDirNormalize = _bodyDir.toVector2().Normalize();
            Vector2 bodyRightDirNormalize = _bodyRightDir.toVector2().Normalize();

            float offsetValueY = Vector2.Dot(bodyDirNormalize, targetDirNormalize);//Y轴的偏移值，判断前后（点乘只能判断前后）
            float offsetValueX = Vector2.Dot(bodyRightDirNormalize, targetDirNormalize);

            if (offsetValueY < 0.3)
            {
                float maxSpeed;
                if (offsetValueX == 0)
                    maxSpeed = _maxSpeed;
                else
                    maxSpeed = _maxTurnSpeed;

                float speedPercent = _realSpeed / maxSpeed;

                if (speedPercent > 0.5)
                    offsetValueY = -1;
                else if (speedPercent < -0.5)
                    offsetValueY = 1;
                else
                    offsetValueY = 0;

                if (offsetValueX > 0)
                    offsetValueX = 1;
                else if (offsetValueX < 0)
                    offsetValueX = -1;
            }

            if (offsetValueX < 0.1 && offsetValueX > -0.1)
                offsetValueX = 0;

            if (_needSlowMove)
                offsetValueY /= 2;

            if (_hasObstruction && offsetValueX != 0)
                offsetValueY = 0;

            userCmd.joystickLeftY = offsetValueY;
            userCmd.joystickLeftX = offsetValueX;

            userCmd.controlType = (int)ControlType.ByTankBody;

            //logger.Error("userCmd.joystickLeftX:{0}, userCmd.joystickLeftY:{1}.", userCmd.joystickLeftX, userCmd.joystickLeftY);
        }

        private void Turn(ref UserCmd userCmd)
        {
            Vector2 targetDirNormalize = (_targetPosition.toVector2() - _bodyPos.toVector2()).Normalize();
            Vector2 bodyRightDirNormalize = _bodyRightDir.toVector2().Normalize();
            
            float offsetValueX = Vector2.Dot(bodyRightDirNormalize, targetDirNormalize);

            userCmd.joystickLeftX = offsetValueX;

            userCmd.controlType = (int)ControlType.ByTankBody;

            //logger.Error("userCmd.joystickLeftX:{0}, userCmd.joystickLeftY:{1}.", userCmd.joystickLeftX, userCmd.joystickLeftY);
        }
    }
}