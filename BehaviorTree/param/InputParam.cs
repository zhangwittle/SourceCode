using BattleServer.World;
using BattleServer.World.AIUseBehaviorTree;
using BattleServer.World.PhysicsRef;
using Framework.Utils;
using IronForce2;
using NLog;
using System;
using System.Collections.Generic;
using System.Numerics;
using static Framework.Utils.CFUtils;

namespace BehaviorTree
{
    public enum AttackTag
    {
        AT_None = 0,
        AT_Move = 1,
        AT_Attack = 2,
        AT_Turn = 4,
    }

    public class InputParam
    {
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private Robot _robot = null;
        //attack target
        private int _attackTargetID = 0;
        private bool _hasAttackTarget = false;
        private bool _focusHaveMostSilverBadgeAttackTarget = false;
        //detect target
        private int _detectTargetAIWayPointID = 0;
        private bool _hasDetectTarget = false;
        //start target
        private int _startTargetID = 0;
        private Vector3 _startTargetPos;
        private bool _hasStartTarget = false;
        private bool _hasFindStartTarget = false;
        //move target
        private int _halfMoveTargetAIWayPointID = 0;
        private int _notMoveTargetAIWayPointID = 0;
        private int _moveTargetAIWayPointID = 0;
        private bool _hasMoveTarget = false;
        private Vector3 _moveTargetPos;
        //dodge target
        private bool _hasDodgeMoveTarget = false;
        private Vector3 _dodgeMoveTargetPos;
        //resource target
        private int _resourceTargetID = 0;
        private bool _hasResourceTarget = false;
        private Vector3 _resourceTargetPos;
        //prop target
        private int _propTargetID = 0;
        private bool _hasPropTarget = false;
        private Vector3 _propTargetPos;
        //cur move target
        private bool _hasCurMoveTarget = false;
        private Vector3 _curMoveTargetPos;
        //attack tag
        private int _attackTag;

        //skill
        private int _attackSkillID;
        private int _moveSkillID;
        private int _healTargetID;

        public int healTargetID { get => _healTargetID; }
        public int moveSkillID { get => _moveSkillID; }
        public int attackSkillID { get => _attackSkillID; }

        private bool _hasObstruction = false;
        private static float _randomAngleMax = 2;
        private static float _randomAngle = Mathf.Random() * _randomAngleMax;

        public Robot robot { get => _robot; }
        public Player robotPlayer { get => _robot.player; }
        public GameplayWorld world { get => _robot.player.world; }
        public int currentFrame { get => _robot.player.world.currentFrame; }
        //attack target
        public int attackTargetID { get => _attackTargetID; }
        public bool hasAttackTarget { get => _hasAttackTarget; }
        public bool focusHaveMostSilverBadgeAttackTarget { get => _focusHaveMostSilverBadgeAttackTarget; }
        //detect target
        public int detectTargetAIWayPointID { get => _detectTargetAIWayPointID; }
        public bool hasDetectTarget { get => _hasDetectTarget; }
        //start target
        public int startTargetID { get => _startTargetID; }
        //move target
        public int halfMoveTargetAIWayPointID { get => _halfMoveTargetAIWayPointID; }
        public int notMoveTargetAIWayPointID { get => _notMoveTargetAIWayPointID; }
        public int moveTargetAIWayPointID { get => _moveTargetAIWayPointID; }
        public bool hasMoveTarget { get => _hasMoveTarget; }
        public Vector3 moveTargetPos { get => _moveTargetPos; }
        //dodge target
        public bool hasDodgeMoveTarget { get => _hasDodgeMoveTarget; }
        public Vector3 dodgeMoveTargetPos { get => _dodgeMoveTargetPos; }
        //resource target
        public int resourceTargetID { get => _resourceTargetID; }
        public bool hasResourceTarget { get => _hasResourceTarget; }
        public Vector3 resourceTargetPos { get => _resourceTargetPos; }
        //prop target
        public int propTargetID { get => _propTargetID; }
        public bool hasPropTarget { get => _hasPropTarget; }
        public Vector3 propTargetPos { get => _propTargetPos; }
        //cur move target
        public bool hasCurMoveTarget { get => _hasCurMoveTarget; }
        public Vector3 curMoveTargetPos { get => _curMoveTargetPos; }
        //attack tag
        public int attackTag { get => _attackTag; set => _attackTag = value; }
        public bool hasObstruction { get => _hasObstruction; }
        public float randomAngle { get => _randomAngle; }

        //public static int testPlayerID = 0;

        public InputParam(Robot robot)
        {
            _robot = robot;
        }

        public void Dispose()
        {
            _robot = null;
        }

        public virtual bool CanHasAttackTag(AttackTag attackTag)
        {
            return true;
        }

        public virtual bool HasAttackTag(AttackTag attackTag)
        {
            return HasAttackTag(_attackTag, attackTag);
        }

        protected bool HasAttackTag(int localAttackTag, AttackTag attackTag)
        {
            if ((localAttackTag & attackTag.ToInt()) == attackTag.ToInt())
                return true;
            return false;
        }

        public bool HasEnemyInRadar(int limitFrame)
        {
            foreach (EnemySignalInfo enemySignalInfo in _robot.robotRadar.enemySignalMap.Values)
            {
                if (!enemySignalInfo.canSee(currentFrame, limitFrame))
                    continue;

                Player enemyPlayer = world.playerController.GetPlayer(enemySignalInfo.playerID);
                if (null == enemyPlayer || !enemyPlayer.isAlive)
                    continue;

                //if (robotPlayer.ID == 2)
                //    logger.Error("true.");
                return true;
            }
            //if (robotPlayer.ID == 2)
            //    logger.Error("false.");
            return false;
        }

        public List<Player> GetEnemyInRadar(int limitFrame)
        {
            List<Player> playerList = new List<Player>();
            foreach (EnemySignalInfo enemySignalInfo in _robot.robotRadar.enemySignalMap.Values)
            {
                if (!enemySignalInfo.canSee(currentFrame, limitFrame))
                    continue;

                Player enemyPlayer = world.playerController.GetPlayer(enemySignalInfo.playerID);
                if (null == enemyPlayer || !enemyPlayer.isAlive)
                    continue;

                playerList.Add(enemyPlayer);
            }
            return playerList;
        }

        class AttackTargetInfo : IComparable<AttackTargetInfo>
        {
            public enum DistanceIntervalID
            {
                DI_Near,
                DI_Middle,
                DI_Far,
            }

            private Player _targetPlayer = null;
            private bool _targetHasAimObstacle = true;
            private float _targetHpPercent = 1;
            private float _distance = float.MaxValue;
            private DistanceIntervalID _distanceIntervalID = DistanceIntervalID.DI_Far;
            private float _self2TargetTurretDegle = PhysicsUtil.HALF_MAX_DEGLE;
            private float _target2SelfTurretDegle = PhysicsUtil.HALF_MAX_DEGLE;
            private float _weight = 0;

            private float _self2TargetTurretDegleWeight = 1;
            private float _targetHPWeight = 1;
            private float _target2SelfTurretDegleWeight = 1;

            public Player targetPlayer { get => _targetPlayer; }
            public bool targetHasAimObstacle { get => _targetHasAimObstacle; }
            public float targetHpPercent { get => _targetHpPercent; }
            public float distance { get => _distance; }
            public DistanceIntervalID distanceIntervalID { get => _distanceIntervalID; }
            public float self2TargetTurretDegle { get => _self2TargetTurretDegle; }
            public float target2SelfTurretDegle { get => _target2SelfTurretDegle; }
            public float weight { get => _weight; }

            public AttackTargetInfo(Player targetPlayer, bool targetHasAimObstacle, float targetHpPercent, float distance, float self2TargetTurretDegle, float target2SelfTurretDegle)
            {
                _targetPlayer = targetPlayer;
                _targetHasAimObstacle = targetHasAimObstacle;
                _targetHpPercent = targetHpPercent;
                _distance = distance;
                _distanceIntervalID = GetDistanceIntervalID();
                _self2TargetTurretDegle = self2TargetTurretDegle;
                _target2SelfTurretDegle = target2SelfTurretDegle;
                _weight = _self2TargetTurretDegleWeight * _self2TargetTurretDegle / PhysicsUtil.HALF_MAX_DEGLE + _targetHPWeight * _targetHpPercent +
                    _target2SelfTurretDegleWeight * _target2SelfTurretDegle / PhysicsUtil.HALF_MAX_DEGLE;
            }

            private DistanceIntervalID GetDistanceIntervalID()
            {
                if (_distance < 20)
                    return DistanceIntervalID.DI_Near;
                if (_distance < 50)
                    return DistanceIntervalID.DI_Middle;
                return DistanceIntervalID.DI_Far;
            }

            public int CompareTo(AttackTargetInfo other)
            {
                if (_targetHasAimObstacle != other.targetHasAimObstacle)
                {
                    if (_targetHasAimObstacle)
                        return 1;
                    return 0;
                }

                if (_distanceIntervalID != other.distanceIntervalID)
                {
                    if (_distanceIntervalID > other.distanceIntervalID)
                        return 1;
                    return 0;
                }

                if (_weight < other.weight)
                    return 1;
                return 0;
            }
        }

        public void FindHealTarget()
        {
            int healSkill = robotPlayer.SkillManager.HealSkillReady();
            if (healSkill == 0)
            {
                _healTargetID = 0;
                return;
            }
               
            if (_healTargetID > 0)
            {
                Player targetPlayer = robotPlayer.world.playerController.GetPlayer(_healTargetID);
                if (targetPlayer == null || !targetPlayer.isAlive)
                {
                    _healTargetID = 0;
                }
                else
                {
                    float distance = Vector3.Distance(robotPlayer.Position, targetPlayer.Position);
                    float hpPercent = targetPlayer.numericalObject.HP.ToFloat() / targetPlayer.numericalObject.GetTankProperty(TankProperty.hp);
                    bool hasAimObstacle = HasAimObstacle(targetPlayer);

                    if (distance > 25 || hpPercent > 0.9f || hasAimObstacle)
                    {
                        _healTargetID = 0;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            foreach (Player player in robotPlayer.world.playerController.players.Values)
            {
                if (player == null || !player.isAlive || _robot.player == player || !Player.IsSameTeam(player, robotPlayer))
                    continue;

                float distance = Vector3.Distance(robotPlayer.Position, player.Position);
                if (distance > 20)
                    continue;

                float hpPercent = player.numericalObject.HP.ToFloat() / player.numericalObject.GetTankProperty(TankProperty.hp);

                if (hpPercent > 0.8f)
                    continue;

                bool hasAimObstacle = HasAimObstacle(player);
                if (hasAimObstacle)
                    continue;

                _healTargetID = player.ID;
                return;
            }

            _healTargetID = 0;
        }
        
        public void FindAttackTarget(int limitFrame, bool findHaveMostSilverBadgeAttackTarget, int maxFocusRobotNum)
        {
            AttackTargetInfo attackTargetInfo = null;

            if (findHaveMostSilverBadgeAttackTarget && world.statisticObject.battleType == BattleTypeConfig.supportControlBattleType)
            {
                attackTargetInfo = FindHaveMostSilverBadgeAttackTarget(maxFocusRobotNum);
            }

            if (attackTargetInfo == null)
            {
                attackTargetInfo = FindCanSeeAttackTarget(limitFrame, maxFocusRobotNum);
            }

            if (null != attackTargetInfo)
            {
                SetAttackTarget(attackTargetInfo.targetPlayer);
            }
            else
            {
                ClearAttackSkill();
                ClearAttackTarget();
            }
        }

        private static int focusHaveMostSilverBadgeAttackTargetNum = 5;
        private AttackTargetInfo FindHaveMostSilverBadgeAttackTarget(int maxFocusRobotNum)
        {
            //logger.Error("FindHaveMostSilverBadgeAttackTarget.");
            int enemyTeamType = TEAM_TYPE.GetEnemyTeamType(robotPlayer.battleTeamType);
            List<Player> haveMostSilverBadgePlayers = world.playerController.GetHaveMostSilverBadgePlayers();
            foreach (Player player in haveMostSilverBadgePlayers)
            {
                int sliverBadgePropValue = player.statisticObject.sliverBadgePropValue;
                if (sliverBadgePropValue < focusHaveMostSilverBadgeAttackTargetNum)
                {
                    _focusHaveMostSilverBadgeAttackTarget = false;
                    return null;
                }
                if (enemyTeamType != player.battleTeamType)
                {
                    continue;
                }
                if (player.GetFocusOnMeRobotNum() >= maxFocusRobotNum)
                {
                    if (!hasAttackTarget || _attackTargetID != player.playerID)
                    {
                        continue;
                    }
                }
                //logger.Error("FindHaveMostSilverBadgeAttackTarget succeed. playerID:{0}, teamType:{1}, isRobot:{2}, silverBadgeNum:{3}, selfTeamType:{4}, selfPlayerID:{5}.",
                //    player.playerID, player.battleTeamType, player.isRobot, sliverBadgePropValue, robotPlayer.battleTeamType, robotPlayer.playerID);
                _focusHaveMostSilverBadgeAttackTarget = true;
                AttackTargetInfo attackTargetInfo = new AttackTargetInfo(player, false, 1, 0, 0, 0);
                return attackTargetInfo;
            }
            _focusHaveMostSilverBadgeAttackTarget = false;
            return null;
        }

        private AttackTargetInfo FindCanSeeAttackTarget(int limitFrame, int maxFocusRobotNum)
        {
            AttackTargetInfo attackTargetInfo = null;
            List<EnemySignalInfo> canSeeEnemySignalInfos = new List<EnemySignalInfo>();
            List<EnemySignalInfo> noTooMuchFocusEnemySignalInfos = new List<EnemySignalInfo>();
            foreach (EnemySignalInfo enemySignalInfo in _robot.robotRadar.enemySignalMap.Values)
            {
                if (!enemySignalInfo.canSee(currentFrame, limitFrame))
                {
                    continue;
                }
                Player enemyPlayer = world.playerController.GetPlayer(enemySignalInfo.playerID);
                if (null == enemyPlayer || _robot.player == enemyPlayer || !enemyPlayer.isAlive)
                {
                    continue;
                }
                canSeeEnemySignalInfos.Add(enemySignalInfo);
                if (enemyPlayer.GetFocusOnMeRobotNum() >= maxFocusRobotNum)
                {
                    if (!hasAttackTarget || _attackTargetID != enemyPlayer.playerID)
                    {
                        continue;
                    }
                }
                noTooMuchFocusEnemySignalInfos.Add(enemySignalInfo);
            }
            List<EnemySignalInfo> enemySignalInfos = null;
            if (noTooMuchFocusEnemySignalInfos.Count > 0)
            {
                enemySignalInfos = noTooMuchFocusEnemySignalInfos;
            }
            else
            {
                enemySignalInfos = canSeeEnemySignalInfos;
            }
            foreach (EnemySignalInfo enemySignalInfo in enemySignalInfos)
            {
                Player enemyPlayer = world.playerController.GetPlayer(enemySignalInfo.playerID);
                bool hasAimObstacle = HasAimObstacle(enemyPlayer);
                float hpPercent = enemyPlayer.numericalObject.HP.ToFloat() / enemyPlayer.numericalObject.GetTankProperty(TankProperty.hp);
                float distance = Vector3.Distance(robotPlayer.Position, enemyPlayer.Position);
                float self2TargetTurretDegle = RayCastManager.GetTargetAngleDiff(robotPlayer.physicalObject.TurretPosition, enemyPlayer.Position, robotPlayer.physicalObject.TurretDirection);
                float target2SelfTurretDegle = RayCastManager.GetTargetAngleDiff(enemyPlayer.physicalObject.TurretPosition, robotPlayer.Position, enemyPlayer.physicalObject.TurretDirection);

                AttackTargetInfo tempAttackTargetInfo = new AttackTargetInfo(enemyPlayer, hasAimObstacle, hpPercent, distance, self2TargetTurretDegle, target2SelfTurretDegle);
                if (attackTargetInfo != null && 1 != attackTargetInfo.CompareTo(tempAttackTargetInfo))
                {
                    continue;
                }
                attackTargetInfo = tempAttackTargetInfo;
            }
            return attackTargetInfo;
        }

        private void SetAttackTarget(Player player)
        {
            RemoveOldAttackTargetFocusOn();
            _attackTargetID = player.playerID;
            _hasAttackTarget = true;
            player.AddFocusOnMeRobotPlayerID(robotPlayer.playerID);
        }

        private void RemoveOldAttackTargetFocusOn()
        {
            if (!_hasAttackTarget)
            {
                return;
            }
            Player oldTarget = world.playerController.GetPlayer(_attackTargetID);
            if (oldTarget == null)
            {
                return;
            }
            oldTarget.RemoveFocusOnMeRobotPlayerID(robotPlayer.playerID);
        }

        public Player GetAttackTarget()
        {
            if (!_hasAttackTarget)
                return null;

            Player player = world.playerController.GetPlayer(_attackTargetID);
            if (player != null && player.isAlive)
                return player;

            ClearAttackSkill();
            ClearAttackTarget();
            return null;
        }

        public void ClearMoveSkill()
        {
            _moveSkillID = 0;
        }
        public void ClearAttackSkill()
        {
            _attackSkillID = 0;
        }

        public void ClearHealSkill()
        {
            _healTargetID = 0;
        }

        public void ClearAttackTarget()
        {
            RemoveOldAttackTargetFocusOn();
            _hasAttackTarget = false;
            _attackTargetID = 0;
            _focusHaveMostSilverBadgeAttackTarget = false;
        }

        public void MoveStepAttackTarget()
        {
            Player player = GetAttackTarget();
            if (player != null)
            {
                _robot.robotNavPath.Step(player, out _hasObstruction);
                Move2CurMoveTarget();

                //_robot.robotNavPath.SendPathDebugInfo();
            }
            else
                ClearCurMoveTarget();
        }

        public void FindDetectTarget()
        {
            int battleTeamType = robotPlayer.battleTeamType;
            float selfHPRatio = robotPlayer.numericalObject.HP.ToFloat() / robotPlayer.numericalObject.GetTankProperty(TankProperty.hp);

            AIWayPoint nearestAIWayPoint = GetNearestAIWayPoint();
            if (nearestAIWayPoint == null)
            {
                logger.Debug("FindDetectTarget no nearestAIWayPoint.");
                ClearDetectTarget();
                return;
            }
            List<WeightRandomInfo> linkedAIWayPointWeightRandomInfoList = new List<WeightRandomInfo>();
            foreach (int aiWayPointID in nearestAIWayPoint.linkedAIWayPointIDSet)
            {
                AIWayPoint aiWayPoint = world.map.mapInfo.aiWayPoints[aiWayPointID];
                Vector3 aiWayPointPostion = aiWayPoint.position;

                if (!_robot.robotRadar.DetectVisible(robotPlayer, aiWayPointPostion))
                    continue;

                float tacticalValue = world.influenceMap.GetTacticalValue(aiWayPointPostion);
                float dominationIncome = world.influenceMap.GetDominationIncome(aiWayPointPostion, battleTeamType);
                float killIncome = world.influenceMap.GetKillIncome(aiWayPointPostion, battleTeamType);
                float propIncome = world.influenceMap.GetPropIncome(aiWayPointPostion);
                float safetyValue = world.influenceMap.GetSafetyValue(aiWayPointPostion, battleTeamType);
                float threatValue = world.influenceMap.GetThreatValue(aiWayPointPostion, battleTeamType);

                float floatWeight = tacticalValue + dominationIncome + killIncome + propIncome + safetyValue - threatValue * selfHPRatio;
                int weight = Mathf.Ceil(floatWeight * WeightRandomInfo.FLOAT2INT_WEIGHT).ToInt();
                if (weight < 0)
                {
                    weight = 0;
                }

                WeightRandomInfo weightRandomInfo = new WeightRandomInfo(aiWayPointID, weight);
                linkedAIWayPointWeightRandomInfoList.Add(weightRandomInfo);
            }

            WeightRandomInfo selectWeightRandomInfo = WeightRandom(linkedAIWayPointWeightRandomInfoList);

            if (selectWeightRandomInfo == null)
                ClearDetectTarget();
            else
            {
                _hasDetectTarget = true;
                _detectTargetAIWayPointID = selectWeightRandomInfo.id;
            }

            //if (robotPlayer.ID == 2)
            //    logger.Error("_detectTargetAIWayPointID:{0}.", _detectTargetAIWayPointID);
        }

        public bool IsDetectTargetValid()
        {
            //if (robotPlayer.ID == 2)
            //    logger.Error(".");

            if (!_hasDetectTarget)
                return false;

            if (_hasAttackTarget || _hasDodgeMoveTarget)
                return false;

            AIWayPoint aiWayPoint = world.map.mapInfo.aiWayPoints[_detectTargetAIWayPointID];
            if (aiWayPoint == null)
                return false;

            if (!_robot.robotRadar.DetectVisible(robotPlayer, aiWayPoint.position))
                return false;

            if (GetDetectErrorDegleFromBody() >= PhysicsUtil.QUARTER_MAX_DEGLE)
                return false;

            return true;
        }

        public void ClearDetectTarget()
        {
            _hasDetectTarget = false;
            _detectTargetAIWayPointID = 0;
            //if (robotPlayer.ID == 2)
            //    logger.Error(".");
        }
        
        public void FindPatrolTarget(bool findStartTarget)
        {
            if (findStartTarget && !_hasFindStartTarget)
            {
                FindStartPointPatrolTarget();
                _hasFindStartTarget = true;
            }
            else
            {
                FindWayPointPatrolTarget();
            }
        }

        private const int _oneStartTargetMaxRobot = 2;
        private void FindStartPointPatrolTarget()
        {
            List<WeightRandomInfo> weightRandomInfoList = new List<WeightRandomInfo>();
            foreach (AITacticalPoint aITacticalPoint in world.map.mapInfo.aiTacticalPoints.Values)
            {
                int weight = Mathf.Ceil(aITacticalPoint.tacticalMax * WeightRandomInfo.FLOAT2INT_WEIGHT).ToInt();
                if (weight < 0)
                {
                    weight = 0;
                }
                WeightRandomInfo weightRandomInfo = new WeightRandomInfo(aITacticalPoint.id, weight);
                weightRandomInfoList.Add(weightRandomInfo);
            }

            List<WeightRandomInfo> canUseWeightRandomInfoList = new List<WeightRandomInfo>();
            foreach (WeightRandomInfo weightRandomInfo in weightRandomInfoList)
            {
                bool notMove = false;
                int useThisStartTargetRobotNum = 0;
                foreach (Robot tempRobot in world.playerController.robotManager.robotList)
                {
                    if (TEAM_TYPE.GetFriendTeamType(robotPlayer.battleTeamType) != tempRobot.player.battleTeamType)
                        continue;
                    if (tempRobot.AI.inputParam.startTargetID != weightRandomInfo.id)
                        continue;
                    useThisStartTargetRobotNum++;
                    if (useThisStartTargetRobotNum < _oneStartTargetMaxRobot)
                        continue;
                    notMove = true;
                    break;
                }
                if (notMove)
                    continue;
                canUseWeightRandomInfoList.Add(weightRandomInfo);
            }

            WeightRandomInfo selectWeightRandomInfo = null;
            if (canUseWeightRandomInfoList.Count > 0)
                selectWeightRandomInfo = WeightRandom(canUseWeightRandomInfoList);
            else
                selectWeightRandomInfo = WeightRandom(weightRandomInfoList);

            if (selectWeightRandomInfo == null)
            {
                ClearStartTarget();
            }
            else
            {
                _hasStartTarget = true;
                _startTargetID = selectWeightRandomInfo.id;
                _startTargetPos = world.map.mapInfo.aiTacticalPoints[_startTargetID].position;
                float randomX = RandomUtils.NextFloat() * 2 - 1;
                float randomZ = RandomUtils.NextFloat() * 2 - 1;
                Vector3 randomVector = new Vector3(randomX, 0, randomZ).Normalize();
                int randomDistance = RandomUtils.NextInt(15);
                randomVector *= randomDistance;
                _startTargetPos += randomVector;

                //if (testPlayerID == 0)
                //    testPlayerID = robotPlayer.ID;
                //if (testPlayerID == robotPlayer.ID)
                //    logger.Error("FindStartPointPatrolTarget succeed. aITacticalPointID:{0}.", _startTargetID);
            }
        }

        public void ClearStartTarget()
        {
            _startTargetID = 0;
            _hasStartTarget = false;
        }

        private void FindWayPointPatrolTarget()
        {
            int battleTeamType = robotPlayer.battleTeamType;
            float selfHPRatio = robotPlayer.numericalObject.HP.ToFloat() / robotPlayer.numericalObject.GetTankProperty(TankProperty.hp);

            AIWayPoint nearestAIWayPoint = GetNearestAIWayPoint();
            if (nearestAIWayPoint == null)
            {
                logger.Debug("FindWayPointPatrolTarget no nearestAIWayPoint.");
                ClearMoveTarget();
                return;
            }
            HashSet<int> canMoveAIWayPointIDSet = new HashSet<int>();
            foreach (int aiWayPointID in nearestAIWayPoint.linkedAIWayPointIDSet)
            {
                bool notMove = false;
                foreach (Robot tempRobot in world.playerController.robotManager.robotList)
                {
                    if (TEAM_TYPE.GetFriendTeamType(robotPlayer.battleTeamType) != tempRobot.player.battleTeamType)
                    {
                        continue;
                    }
                    if (tempRobot.AI.inputParam.notMoveTargetAIWayPointID != aiWayPointID)
                    {
                        continue;
                    }
                    notMove = true;
                    break;
                }
                if (notMove)
                    continue;
                canMoveAIWayPointIDSet.Add(aiWayPointID);
            }

            if (canMoveAIWayPointIDSet.Count == 0)
            {
                canMoveAIWayPointIDSet = nearestAIWayPoint.linkedAIWayPointIDSet;
            }

            List<WeightRandomInfo> linkedAIWayPointWeightRandomInfoList = new List<WeightRandomInfo>();
            foreach (int aiWayPointID in canMoveAIWayPointIDSet)
            {
                AIWayPoint aiWayPoint = world.map.mapInfo.aiWayPoints[aiWayPointID];
                Vector3 aiWayPointPostion = aiWayPoint.position;

                float tacticalValue = world.influenceMap.GetTacticalValue(aiWayPointPostion);
                float dominationIncome = world.influenceMap.GetDominationIncome(aiWayPointPostion, battleTeamType);
                float killIncome = world.influenceMap.GetKillIncome(aiWayPointPostion, battleTeamType);
                float propIncome = world.influenceMap.GetPropIncome(aiWayPointPostion);
                float safetyValue = world.influenceMap.GetSafetyValue(aiWayPointPostion, battleTeamType);
                float threatValue = world.influenceMap.GetThreatValue(aiWayPointPostion, battleTeamType);
                float sliverBadgePrePropIncome = world.influenceMap.GetSliverBadgePrePropIncome(aiWayPointPostion);

                float floatWeight = tacticalValue + dominationIncome + killIncome + propIncome + safetyValue - threatValue * selfHPRatio + sliverBadgePrePropIncome;
                int weight = Mathf.Ceil(floatWeight * WeightRandomInfo.FLOAT2INT_WEIGHT).ToInt();
                
                if (aiWayPointID == _halfMoveTargetAIWayPointID)
                    weight /= 2;

                if (weight < 1)
                    weight = 1;

                WeightRandomInfo weightRandomInfo = new WeightRandomInfo(aiWayPointID, weight);
                linkedAIWayPointWeightRandomInfoList.Add(weightRandomInfo);
            }

            Dictionary<int, List<WeightRandomInfo>> aiWayPointLinkGroupMap = new Dictionary<int, List<WeightRandomInfo>>();
            int curGroupIndex = 1;
            foreach (WeightRandomInfo linkedAIWayPointWeightRandomInfo in linkedAIWayPointWeightRandomInfoList)
            {
                AIWayPoint linkedAIWayPoint = world.map.mapInfo.aiWayPoints[linkedAIWayPointWeightRandomInfo.id];
                int willJoinGroupIndex = 0;
                List<int> needMegerGroupIndex = new List<int>();
                foreach (KeyValuePair<int, List<WeightRandomInfo>> weightRandomInfoGroupPair in aiWayPointLinkGroupMap)
                {
                    int groupIndex = weightRandomInfoGroupPair.Key;
                    List<WeightRandomInfo> weightRandomInfoGroup = weightRandomInfoGroupPair.Value;

                    foreach (WeightRandomInfo inGroupWeightRandomInfo in weightRandomInfoGroup)
                    {
                        AIWayPoint inGroupLinkedAIWayPoint = world.map.mapInfo.aiWayPoints[inGroupWeightRandomInfo.id];
                        if (linkedAIWayPoint.IsLinkedAtOnePointAIWayPoint(inGroupLinkedAIWayPoint))
                        {
                            if (willJoinGroupIndex == 0)
                                willJoinGroupIndex = groupIndex;
                            else
                                needMegerGroupIndex.Add(groupIndex);
                            break;
                        }
                        else
                            continue;
                    }
                }

                if (willJoinGroupIndex != 0)
                {
                    aiWayPointLinkGroupMap[willJoinGroupIndex].Add(linkedAIWayPointWeightRandomInfo);

                    foreach (int groupIndex in needMegerGroupIndex)
                    {
                        foreach (WeightRandomInfo inGroupWeightRandomInfo in aiWayPointLinkGroupMap[groupIndex])
                            aiWayPointLinkGroupMap[willJoinGroupIndex].Add(inGroupWeightRandomInfo);
                        aiWayPointLinkGroupMap.Remove(groupIndex);
                    }

                    continue;
                }

                List<WeightRandomInfo> newWeightRandomInfoGroup = new List<WeightRandomInfo>();
                newWeightRandomInfoGroup.Add(linkedAIWayPointWeightRandomInfo);
                aiWayPointLinkGroupMap.Add(curGroupIndex++, newWeightRandomInfoGroup);
            }

            foreach (List<WeightRandomInfo> weightRandomInfoGroup in aiWayPointLinkGroupMap.Values)
            {
                foreach (WeightRandomInfo inGroupWeightRandomInfo in weightRandomInfoGroup)
                    inGroupWeightRandomInfo.weight /= weightRandomInfoGroup.Count;
            }

            WeightRandomInfo selectWeightRandomInfo = WeightRandom(linkedAIWayPointWeightRandomInfoList);

            if (selectWeightRandomInfo == null)
                ClearMoveTarget();
            else
            {
                _hasMoveTarget = true;

                if (_notMoveTargetAIWayPointID != 0)
                    _halfMoveTargetAIWayPointID = _notMoveTargetAIWayPointID;
                _notMoveTargetAIWayPointID = _moveTargetAIWayPointID;

                _moveTargetAIWayPointID = selectWeightRandomInfo.id;
                _moveTargetPos = world.map.mapInfo.aiWayPoints[_moveTargetAIWayPointID].position;
                float randomX = RandomUtils.NextFloat() * 2 - 1;
                float randomZ = RandomUtils.NextFloat() * 2 - 1;
                Vector3 randomVector = new Vector3(randomX, 0, randomZ).Normalize();
                int randomDistance = RandomUtils.NextInt(15);
                randomVector *= randomDistance;
                _moveTargetPos += randomVector;

                //if (testPlayerID == robotPlayer.ID)
                //{
                //    logger.Error("FindWayPointPatrolTarget succeed. moveTargetAIWayPointID:{0}, notMoveTargetAIWayPointID:{1}, halfMoveTargetAIWayPointID:{2}.", 
                //        _moveTargetAIWayPointID, _notMoveTargetAIWayPointID, _halfMoveTargetAIWayPointID);
                //}
            }
        }

        public void ClearNotAndHalfMoveTarget()
        {
            _halfMoveTargetAIWayPointID = 0;
            _notMoveTargetAIWayPointID = 0;

            //if (robotPlayer.ID == 2)
            //    logger.Error(".");
        }

        public void ClearMoveTarget()
        {
            _moveTargetAIWayPointID = 0;
            _hasMoveTarget = false;

            //if (robotPlayer.ID == 2)
            //    logger.Error(".");
        }

        public void MoveStepMoveTarget()
        {
            if (_hasStartTarget)
            {
                //if (testPlayerID == robotPlayer.ID)
                //    logger.Error("MoveStepMoveTarget _hasStartTarget. _testPlayerID:{0}, _startTargetPos:{1}.", testPlayerID, _startTargetPos);
                _robot.robotNavPath.Step(CFUtils.toVector3(_startTargetPos), out _hasObstruction);
                Move2CurMoveTarget();
            }
            else if (_hasMoveTarget)
            {
                //if (testPlayerID == robotPlayer.ID)
                //    logger.Error("Move2CurMoveTarget _hasMoveTarget. _testPlayerID:{0}, _moveTargetPos:{1}.", testPlayerID, _moveTargetPos);
                _robot.robotNavPath.Step(CFUtils.toVector3(_moveTargetPos), out _hasObstruction);
                Move2CurMoveTarget();
            }
            else
                ClearCurMoveTarget();
        }

        public void FindRandomDodgePostion()
        {
            int battleTeamType = robotPlayer.battleTeamType;

            List<Vector3> posList = _robot.robotRadar.GetRandomCanUsePositionList(robotPlayer.Position);
            if (posList.Count != 0)
            {
                List<WeightRandomInfo> weightRandomInfoList = new List<WeightRandomInfo>();
                for (int i = 0; i < posList.Count; i++)
                {
                    int tempID = i + 1;
                    Vector3 pos = posList[i];

                    float threatValue = world.influenceMap.GetThreatValue(pos, battleTeamType);
                    int weight = Mathf.Ceil(threatValue * WeightRandomInfo.FLOAT2INT_WEIGHT).ToInt();

                    //if (robotPlayer.ID == 2)
                    //    logger.Error("ID:{0}, weight{1}.", tempID, weight);

                    WeightRandomInfo weightRandomInfo = new WeightRandomInfo(tempID, weight);
                    weightRandomInfoList.Add(weightRandomInfo);
                }

                WeightRandomInfo selectWeightRandomInfo = WeightMin(weightRandomInfoList);
                if (selectWeightRandomInfo == null)
                    ClearDodgeMoveTarget();
                else
                {
                    _dodgeMoveTargetPos = posList[selectWeightRandomInfo.id - 1];
                    _hasDodgeMoveTarget = true;

                    //if (robotPlayer.ID == 2)
                    //{
                    //    logger.Error("final ID:{0}, robotPos:<{1}, {2}>, dodgePos:<{3}, {4}>.", 
                    //        id, robotPlayer.Position.X, robotPlayer.Position.Z, _dodgeMoveTargetPos.X, _dodgeMoveTargetPos.Z);
                    //}
                }
            }
            else
                ClearDodgeMoveTarget();
        }

        public void ClearDodgeMoveTarget()
        {
            //if (robotPlayer.ID == 2)
            //    logger.Error("");
            _hasDodgeMoveTarget = false;
        }

        public void MoveStep2DodgePostion()
        {
            if (!_hasDodgeMoveTarget)
                ClearCurMoveTarget();

            bool hasObstruction = false;
            bool isFindPathSucceed = _robot.robotNavPath.Step(CFUtils.toVector3(_dodgeMoveTargetPos), out hasObstruction, false);
            if (!isFindPathSucceed)
            {
                ClearDodgeMoveTarget();
                return;
            }

            Move2CurMoveTarget();

            //_robot.robotNavPath.SendPathDebugInfo();
        }

        public void FindRandomResourcePostion()
        {
            Vector3 robotPosition = robotPlayer.Position;
            int battleTeamType = robotPlayer.battleTeamType;

            List<WeightRandomInfo> weightRandomInfoList = new List<WeightRandomInfo>();
            foreach (ResourceNode resourceNode in world.entityController.ResourceOrganizer.ResourceNodeList)
            {
                float dominationIncome = world.influenceMap.GetDominationIncome(robotPosition, battleTeamType, resourceNode, 0.8f);
                int weight = Mathf.Ceil(dominationIncome * WeightRandomInfo.FLOAT2INT_WEIGHT).ToInt();

                WeightRandomInfo weightRandomInfo = new WeightRandomInfo(resourceNode.ID, weight);
                weightRandomInfoList.Add(weightRandomInfo);
            }

            WeightRandomInfo selectWeightRandomInfo = WeightMax(weightRandomInfoList);
            if (selectWeightRandomInfo == null)
                ClearResourceTarget();
            else
            {
                _resourceTargetID = selectWeightRandomInfo.id;
                _hasResourceTarget = true;
                ResourceNode resourceNode = (ResourceNode)world.entityController.ResourceOrganizer.GetEntity(_resourceTargetID);
                _resourceTargetPos = resourceNode.Position;
            }
        }

        public bool IsResourceTargetVaild()
        {
            ResourceNode resourceNode = (ResourceNode)world.entityController.ResourceOrganizer.GetEntity(_resourceTargetID);
            if (resourceNode.OwnerTeamType == TEAM_TYPE.GetFriendTeamType(robotPlayer.battleTeamType))
                return false;
            return true;
        }

        public bool IsInResourceTarget()
        {
            ResourceNode resourceNode = (ResourceNode)world.entityController.ResourceOrganizer.GetEntity(_resourceTargetID);
            if (RayCastManager.detectCollision(resourceNode, robotPlayer))
                return true;
            return false;
        }

        public void ClearResourceTarget()
        {
            //if (robotPlayer.ID == 2)
            //    logger.Error("");
            _resourceTargetID = 0;
            _hasResourceTarget = false;
        }

        public void MoveStep2ResourcePostion()
        {
            if (!_hasResourceTarget)
                ClearCurMoveTarget();

            if (!IsResourceTargetVaild())
            {
                ClearResourceTarget();
                ClearCurMoveTarget();
                return;
            }

            _robot.robotNavPath.Step(CFUtils.toVector3(_resourceTargetPos), out _hasObstruction);
            Move2CurMoveTarget();

            //_robot.robotNavPath.SendPathDebugInfo();
        }

        public void FindSliverBadgePropPostion()
        {
            foreach (Prop prop in world.scriptManager.propList)
            {
                if (prop.propType != PROP_TYPE.SILVER_BADGE)
                    continue;

                _propTargetID = prop.ID;
                _hasPropTarget = true;
                _propTargetPos = prop.Position;
                return;
            }

            ClearPropTarget();
        }

        public void FindRandomPropPostion()
        {
            Vector3 robotPosition = robotPlayer.Position;

            List<WeightRandomInfo> weightRandomInfoList = new List<WeightRandomInfo>();
            foreach (Prop prop in world.scriptManager.propList)
            {
                float propIncome = world.influenceMap.GetPropIncome(robotPosition, prop, 0.8f);
                int weight = Mathf.Ceil(propIncome * WeightRandomInfo.FLOAT2INT_WEIGHT).ToInt();

                WeightRandomInfo weightRandomInfo = new WeightRandomInfo(prop.ID, weight);
                weightRandomInfoList.Add(weightRandomInfo);
            }

            WeightRandomInfo selectWeightRandomInfo = WeightMax(weightRandomInfoList);
            if (selectWeightRandomInfo == null)
                ClearPropTarget();
            else
            {
                _propTargetID = selectWeightRandomInfo.id;
                _hasPropTarget = true;
                Prop prop = world.scriptManager.GetPropByID(_propTargetID);
                _propTargetPos = prop.Position;
            }
        }

        public bool IsPropTargetExist()
        {
            if (world.scriptManager.GetPropByID(_propTargetID) == null)
                return false;
            return true;
        }

        public void ClearPropTarget()
        {
            //if (robotPlayer.ID == 2)
            //    logger.Error("");
            _propTargetID = 0;
            _hasPropTarget = false;
        }

        public void MoveStep2PropPostion()
        {
            if (!_hasPropTarget)
                ClearCurMoveTarget();

            if (!IsPropTargetExist())
            {
                ClearPropTarget();
                ClearCurMoveTarget();
                return;
            }

            _robot.robotNavPath.Step(CFUtils.toVector3(_propTargetPos), out _hasObstruction);
            Move2CurMoveTarget();

            //_robot.robotNavPath.SendPathDebugInfo();
        }

        public void ClearCurMoveTarget()
        {
            _hasCurMoveTarget = false;
            _hasObstruction = false;
            //if (testPlayerID == robotPlayer.ID)
            //    logger.Error("ClearCurMoveTarget. _testPlayerID:{0}, _hasCurMoveTarget:{1}.", testPlayerID, _hasCurMoveTarget);
        }

        public void Move2CurMoveTarget()
        {
            if (_robot.robotNavPath.HasPath)
            {
                _curMoveTargetPos = _robot.robotNavPath.CurrentPos;
                _hasCurMoveTarget = true;
                //if (testPlayerID == robotPlayer.ID)
                //    logger.Error("Move2CurMoveTarget. _testPlayerID:{0}, _curMoveTargetPos:{1}, _hasCurMoveTarget:{2}.", testPlayerID, _curMoveTargetPos, _hasCurMoveTarget);
            }
            else
                ClearCurMoveTarget();

            //if (robotPlayer.ID == 2)
            //    _robot.robotNavPath.SendPathDebugInfo();
        }

        public void GetTurretPosDir(out Vector3 turretPos, out Vector3 turretDir)
        {
            turretPos = robotPlayer.physicalObject.TurretPosition;
            turretDir = robotPlayer.physicalObject.TurretDirection;
        }

        public void GetBodyPosDir(out Vector3 bodyPos, out Vector3 bodyDir, out Vector3 bodyRightDir)
        {
            bodyPos = robotPlayer.physicalObject.Position;
            bodyDir = robotPlayer.physicalObject.BodyDirection;
            bodyRightDir = robotPlayer.physicalObject.BodyRight;
        }

        private bool HasAimObstacle(Player player)
        {
            return _robot.robotRadar.DetectAimObstacle(player);
        }

        private bool HasAimObstacle(int playerID)
        {
            Player player = world.playerController.GetPlayer(playerID);
            if (null == player)
                return false;
            return HasAimObstacle(player);
        }

        public bool HasAimObstacle()
        {
            return HasAimObstacle(_attackTargetID);
        }

        public bool HasHealTarget()
        {
            return _healTargetID > 0;
        }

        public Player GetHealTarget()
        {    
            Player player = world.playerController.GetPlayer(_healTargetID);
            if (player != null && player.isAlive)
                return player;

            _healTargetID = 0;
            return null;
        }

        public float GetHealDegle()
        {
            Player player = GetHealTarget();
            if (player != null)
                return RayCastManager.GetTargetAngle(CFUtils.toVector3(player.Position), robotPlayer.physicalObject.TurretPosition, Vector2.UnitY);
            return 0;
        }

        public float GetAimDegle()
        {
            Player player = GetAttackTarget();
            if (player != null)
                return RayCastManager.GetTargetAngle(CFUtils.toVector3(player.Position), robotPlayer.physicalObject.TurretPosition, Vector2.UnitY);
            return 0;
        }

        public float GetHealAimingErrorDegle()
        {
            Player player = GetHealTarget();
            if (player != null)
                return RayCastManager.GetTargetAngleDiff(CFUtils.toVector3(player.Position), robotPlayer.physicalObject.TurretPosition, robotPlayer.physicalObject.TurretDirection);
            return PhysicsUtil.HALF_MAX_DEGLE;
        }

        public float GetAimingErrorDegle()
        {
            Player player = GetAttackTarget();
            if (player != null)
                return RayCastManager.GetTargetAngleDiff(CFUtils.toVector3(player.Position), robotPlayer.physicalObject.TurretPosition, robotPlayer.physicalObject.TurretDirection);
            return PhysicsUtil.HALF_MAX_DEGLE;
        }

        public float GetAttackTargetDistance()
        {
            Player player = GetAttackTarget();
            if (player != null)
                return Vector3.Distance(robotPlayer.Position, CFUtils.toVector3(player.Position));
            return 0;
        }

        public float GetDetectDegle()
        {
            if (_hasDetectTarget)
            {
                AIWayPoint aiWayPoint = world.map.mapInfo.aiWayPoints[_detectTargetAIWayPointID];
                if (aiWayPoint == null)
                    return 0;

                return RayCastManager.GetTargetAngle(CFUtils.toVector3(aiWayPoint.position), robotPlayer.physicalObject.TurretPosition, Vector2.UnitY);
            }

            return 0;
        }

        public float GetDetectErrorDegle()
        {
            if (_hasDetectTarget)
            {
                AIWayPoint aiWayPoint = world.map.mapInfo.aiWayPoints[_detectTargetAIWayPointID];
                if (aiWayPoint == null)
                    return PhysicsUtil.HALF_MAX_DEGLE;

                return RayCastManager.GetTargetAngleDiff(CFUtils.toVector3(aiWayPoint.position), robotPlayer.physicalObject.TurretPosition, robotPlayer.physicalObject.TurretDirection);
            }

            return PhysicsUtil.HALF_MAX_DEGLE;
        }

        public float GetDetectErrorDegleFromBody()
        {
            if (_hasDetectTarget)
            {
                AIWayPoint aiWayPoint = world.map.mapInfo.aiWayPoints[_detectTargetAIWayPointID];
                if (aiWayPoint == null)
                    return PhysicsUtil.HALF_MAX_DEGLE;

                return RayCastManager.GetTargetAngleDiff(CFUtils.toVector3(aiWayPoint.position), robotPlayer.physicalObject.TurretPosition, robotPlayer.physicalObject.BodyDirection);
            }

            return PhysicsUtil.HALF_MAX_DEGLE;
        }

        public float GetMoveTargetDistance()
        {
            if (_hasStartTarget)
                return Vector3.Distance(robotPlayer.Position, _startTargetPos);
            return Vector3.Distance(robotPlayer.Position, _moveTargetPos);
        }

        public float GetDodgeTargetDistance()
        {
            return Vector3.Distance(robotPlayer.Position, _dodgeMoveTargetPos);
        }

        public AIWayPoint GetNearestAIWayPoint()
        {
            return world.map.GetNearestAIWayPoint(robotPlayer.Position);
        }

        public bool ReachAIWayPoint()
        {
            if (_hasStartTarget)
                return true;
            if (_moveTargetAIWayPointID == 0)
                return true;
            AIWayPoint nearestAIWayPoint = GetNearestAIWayPoint();
            if (nearestAIWayPoint == null)
            {
                logger.Debug("ReachAIWayPoint no nearestAIWayPoint.");
                return true;
            }
            return _moveTargetAIWayPointID == nearestAIWayPoint.id;
        }

        public bool TargetTankPowerIsSmaller()
        {
            Player targetPlayer = world.playerController.GetPlayer(_attackTargetID);
            if (robotPlayer.tankObject.curTankInfo.tankPower > targetPlayer.tankObject.curTankInfo.tankPower)
                return true;
            return false;
        }

        public bool IsTargetAlive()
        {
            Player targetPlayer = world.playerController.GetPlayer(_attackTargetID);
            if (targetPlayer.isAlive)
                return true;
            return false;
        }

        public bool CanUseAttackSkill()
        {
            if (!hasAttackTarget)
                return false;

            _attackSkillID = robotPlayer.SkillManager.AttackSkillReady(_attackTargetID);

            if(_attackSkillID>0)
                return true;

            return false;
        }

        public bool CanUseMoveSkill()
        {     
            _moveSkillID = robotPlayer.SkillManager.MoveSkillReady();

            if (_moveSkillID > 0)
                return true;

            return false;
        }

        public bool CanFire()
        {
            if (robotPlayer.tankObject.playerMagazine.CanFire())
                return true;
            return false;
        }

        public float GetBodyDegle()
        {
            return RayCastManager.GetTargetAngle(robotPlayer.physicalObject.BodyDirection.toVector2().Normalize(), Vector2.UnitY);
        }

        public void Reset()
        {
            ClearAttackTarget();
            ClearNotAndHalfMoveTarget();
            ClearStartTarget();
            ClearMoveTarget();
            ClearDodgeMoveTarget();
            ClearPropTarget();
            ClearCurMoveTarget();
            ClearAttackSkill();
            ClearMoveSkill();
        }

        public void SetNewRandomAngele(float randomDegleMax)
        {
            _randomAngle = Mathf.Random() * randomDegleMax;
        }

        public bool attackDeldayComplete()
        {
            return robot.curAttackDelayFrame > robot.attackDelayFrame;
        }

        public bool haveFireCount()
        {
            return robot.curFireCountPerRound < robot.fireCountPerRound;
        }

        public void accCurAttackDelay()
        {
            robot.accCurAttackDelay();
        }

        public void accCurFireCount()
        {
            robot.accCurFireCount();
        }

        public void ResetAttackDelay()
        {
            robot.ResetAttackDelay();
        }
    }
}