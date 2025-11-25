using UnityEngine;

public class Player_BasicAttackState : EntityState
{
    private float attackVelocityTimer;//攻击速度计时器
    
    
    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        GenerateAttackVelocity();// 生成并应用攻击时的速度
    }

    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();// 处理攻击时的速度
        
        if(triggerCalled)// 如果触发条件已调用，切换回站立状态
            stateMachine.ChangeState(player.idleState);
    }

    // 处理攻击时的速度，倒计时结束后恢复玩家的垂直速度
    private void HandleAttackVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;// 递减攻击速度计时器（每秒减少时间）
        
        if(attackVelocityTimer < 0)// 如果攻击时间结束，将玩家的水平速度设为 0，保持垂直速度
            player.SetVelocity(0,rb.linearVelocity.y);// 恢复垂直速度，水平速度设为 0
    }

    // 生成攻击时的速度，初始化计时器并设置玩家的攻击速度
    private void GenerateAttackVelocity()
    {
        attackVelocityTimer = player.attackVelocityDuration;// 设置攻击速度持续时间
        player.SetVelocity(player.attackVelocity.x * player.facingDir,player.attackVelocity.y);// 设置攻击时的速度，水平速度由攻击方向控制
    }
}
