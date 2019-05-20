using Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorTree
{
    public class ConditionCanUseMoveSkill : ICondition
    {
        public bool Evaluate(InputParam inputParam)
        {
            bool skill = inputParam.CanUseMoveSkill();
            if (!skill)
                return false;

            float distance = Vector3.Distance(inputParam.curMoveTargetPos, inputParam.robotPlayer.Position);
            if (distance < 7)
                return false;

            Vector2 targetDir = (inputParam.curMoveTargetPos - inputParam.robotPlayer.Position).Normalize().toVector2();
            Vector2 selfDir = inputParam.robotPlayer.physicalObject.GetForwardDir().toVector2();
            float angle = Mathf.Angle(targetDir, selfDir);
            if (angle > 3)
                return false;
            return inputParam.CanUseMoveSkill();
        }
    }
}
