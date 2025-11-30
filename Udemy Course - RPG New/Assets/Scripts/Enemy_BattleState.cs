using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private Transform player;
    private float lastTimeWasInBattle;//上次是在战斗中
    
    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if(player == null)// 如果 player 为 null（还没有检测到玩家），则通过 PlayerDetection 获取玩家的 Transform
            player = enemy.PlayerDetected().transform;

        if (ShouldRetreat())// 如果敌人需要撤退，根据距离判断并设置撤退速度
        {
            rb.linearVelocity = new Vector2(enemy.retrealVelocity.x * -DirectionToPlayer(), enemy.retrealVelocity.y); // 撤退：根据敌人的撤退速度和方向设置敌人的速度
            enemy.HandleFlip(DirectionToPlayer());// 处理敌人的翻转：如果敌人需要撤退，确保它面向撤退的方向
        }
    }

    public override void Update()
    {
        base.Update();
        
        if(enemy.PlayerDetected() == true) // 如果检测到玩家，更新战斗计时器
           UpdateBattleTimer();
        
        if(BattleTimeIsOver())// 如果战斗时间结束，切换到空闲状态
            stateMachine.ChangeState(enemy.idleState);
        
        if(WithinAttackRange() && enemy.PlayerDetected() == true)// 如果玩家在攻击范围内，并且检测到玩家，切换到攻击状态
            stateMachine.ChangeState(enemy.attackState);
        else
            enemy.SetVelocity(enemy.battleMoveSpeed * DirectionToPlayer(),rb.linearVelocity.y);// 如果玩家不在攻击范围内，敌人向玩家移动
        
    }
    
    // 更新战斗计时器，记录上次进入战斗的时间
    private void UpdateBattleTimer() => lastTimeWasInBattle = Time.time;

    // 检查战斗时间是否结束，当前时间与上次战斗开始时间的差值是否超过战斗持续时间
    private bool BattleTimeIsOver() => Time.time > lastTimeWasInBattle + enemy.battleTimeDuration;

    // 判断玩家是否在攻击范围内
    private bool WithinAttackRange() => DistanceToPlayer() < enemy.attackDistance;

    // 判断敌人是否需要撤退，如果玩家距离过近（小于最小撤退距离）
    private bool ShouldRetreat() => DistanceToPlayer() < enemy.minRetreatDistance;

    // 计算敌人与玩家的水平距离
    private float DistanceToPlayer()
    {
        if(player == null) // 如果没有玩家的 Transform，返回最大值
            return float.MaxValue;

        return Mathf.Abs(player.position.x - enemy.transform.position.x);// 返回玩家与敌人之间的水平距离
    }

    // 判断玩家相对于敌人的方向，返回 1 或 -1 来表示向玩家移动的方向
    private int DirectionToPlayer()
    {
        if (player == null)// 如果没有玩家的 Transform，返回 0（表示没有移动）
            return 0;

        return player.position.x > enemy.transform.position.x ? 1 : -1;// 如果玩家在敌人的右侧，返回 1，否则返回 -1
    }
}
