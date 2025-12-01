using UnityEngine;

public class Player_JumpAttackState : PlayerState
{
    private bool touchedGround;//触摸地面
    
    public Player_JumpAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        touchedGround = false;// 重置地面接触标志
        
        player.SetVelocity(player.jumpAttackVelocity.x * player.facingDir,player.jumpAttackVelocity.y);// 设置跳跃攻击的速度，水平速度根据玩家的朝向，垂直速度使用跳跃攻击速度的y分量
    }

    public override void Update()
    {
        base.Update();

        if (player.groundDetected && touchedGround == false)// 如果检测到地面并且玩家尚未接触地面
        {
            touchedGround = true;// 标记为已经接触地面
            anim.SetTrigger("jumpAttackTrigger");// 播放跳跃攻击的触发动画
            player.SetVelocity(0, rb.linearVelocity.y); // 水平速度设置为0，保持垂直速度
        }
        
        if(triggerCalled && player.groundDetected)// 如果触发条件已满足且玩家接触地面，切换回空闲状态
            stateMachine.ChangeState(player.idleState);
    }
}
