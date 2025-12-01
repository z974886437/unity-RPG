using UnityEngine;

public class Player_JumpState : Player_AiredState
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
        
        if(rb.linearVelocity.y < 0 && stateMachine.currentState != player.jumpAttackState) // 如果玩家的垂直速度小于0，且当前状态不是跳跃攻击状态，则说明玩家正在下落
            stateMachine.ChangeState(player.fallState);// 如果玩家在下落，切换到下落状态
    }
}
