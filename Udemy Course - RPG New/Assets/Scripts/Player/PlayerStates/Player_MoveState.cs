using UnityEngine;

public class Player_MoveState : Player_GroundedState
{
    public Player_MoveState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Update()
    {
        base.Update();
        
        if(player.moveInput.x == 0 || player.wallDetected)// 检查玩家是否没有水平输入（x轴）
            stateMachine.ChangeState(player.idleState); // 如果没有输入，切换到idle状态（待机状态）
        
        player.SetVelocity(player.moveInput.x * player.moveSpeed,rb.linearVelocity.y);// 设置角色的速度，水平速度基于玩家的输入和移动速度，垂直速度保持不变（rb.linearVelocity.y）
    }
}
