using UnityEngine;

public class Player_DeadState : PlayerState
{
    public Player_DeadState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        input.Disable();// 禁用输入系统，防止在该状态下接受用户输入
        rb.simulated = false;// 禁用物理模拟，通常用于停止刚体的物理行为（例如跳跃、重力等）
    }
}
