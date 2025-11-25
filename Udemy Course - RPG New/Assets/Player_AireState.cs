using UnityEngine;

public class Player_AireState : EntityState
{
    public Player_AireState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();
        
        if(player.moveInput.x != 0)// 如果玩家有水平输入（左或右），设置水平速度
            player.SetVelocity(player.moveInput.x * (player.moveSpeed * player.inAirMoveMultiplier),rb.linearVelocity.y);// 设置水平速度，根据输入乘以移动速度和空中移动乘数，保持垂直速度不变
    }
}
