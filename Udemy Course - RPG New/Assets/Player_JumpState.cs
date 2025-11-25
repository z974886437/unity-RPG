using UnityEngine;

public class Player_JumpState : EntityState
{
    public Player_JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        player.SetVelocity(rb.linearVelocity.x,player.jumpForce);// 设置玩家的速度，x 轴保持原速度，y 轴设置为跳跃力
    }

    public override void Update()
    {
        base.Update();
        
        if(rb.linearVelocity.y < 0)// 如果玩家的垂直速度小于 0，说明玩家正在向下移动（即下落）
            stateMachine.ChangeState(player.fallState);// 如果玩家在下落，切换到下落状态
    }
}
