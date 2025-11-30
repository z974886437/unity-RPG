using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private Transform player;
    
    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if(player == null)// 如果 player 为 null（还没有检测到玩家），则通过 PlayerDetection 获取玩家的 Transform
            player = enemy.PlayerDetection().transform;
    }

    public override void Update()
    {
        base.Update();
        
        if(WithinAttackRange())// 检查玩家是否在攻击范围内，如果是，则切换到攻击状态
            stateMachine.ChangeState(enemy.attackState);
        else// 如果玩家不在攻击范围内，敌人向玩家移动
            enemy.SetVelocity(enemy.battleMoveSpeed * DirectionToPlayer(),rb.linearVelocity.y);
        
    }

    // 判断玩家是否在攻击范围内
    private bool WithinAttackRange() => DistanceToPlayer() < enemy.attackDistance;

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
