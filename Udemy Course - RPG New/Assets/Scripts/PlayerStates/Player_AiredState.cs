using UnityEngine;

public class Player_AiredState : PlayerState
{
    public Player_AiredState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();
        
        if (player.moveInput.x != 0)// 如果玩家有水平输入（左或右），设置水平速度
            player.SetVelocity(player.moveInput.x * (player.moveSpeed * player.inAirMoveMultiplier),rb.linearVelocity.y);// 设置水平速度，根据输入乘以移动速度和空中移动乘数，保持垂直速度不变
        
        if (input.Player.Attack.WasPressedThisFrame())// 如果玩家按下攻击键，切换到跳跃攻击状态
            stateMachine.ChangeState(player.jumpAttackState);
    }
}
