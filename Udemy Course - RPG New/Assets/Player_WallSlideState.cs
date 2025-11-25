using UnityEngine;

public class Player_WallSlideState : EntityState
{
    public Player_WallSlideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();
        HandleWallSlide(); // 处理墙壁滑行逻辑
        
        if(input.Player.Jump.WasPressedThisFrame())// 如果玩家按下跳跃键，切换到墙壁跳跃状态
            stateMachine.ChangeState(player.wallJumpState);

        if (player.wallDetected == false)// 如果不再接触墙壁，切换到下落状态
            stateMachine.ChangeState(player.fallState);

        if (player.groundDetected)// 如果接触地面，切换到站立状态
        {
            stateMachine.ChangeState(player.idleState); // 切换到站立（Idle）状态
            player.Flip(); // 翻转角色方向
        }
    }

    private void HandleWallSlide()// 处理墙壁滑行逻辑
    {
        if (player.moveInput.y < 0)// 如果玩家向下移动，维持原有的垂直速度
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y);
        else
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y * player.wallSlideSlowMultiplier); // 如果玩家向上移动，减慢垂直速度以模拟墙壁滑行
    }
}
