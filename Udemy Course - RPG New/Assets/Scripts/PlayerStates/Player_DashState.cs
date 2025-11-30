using UnityEngine;

public class Player_DashState : PlayerState
{
    private float originalGravityScale;//原始重力标尺
    private int dashDir;//冲刺方向
    
    public Player_DashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        // 设置冲刺方向和冲刺持续时间
        dashDir = player.moveInput.x != 0 ? ((int)player.moveInput.x) : player.facingDir;; // 根据玩家的朝向设置冲刺方向
        stateTimer = player.dashDuration;// 设置冲刺的持续时间
        
        // 禁用重力，使得冲刺期间不受重力影响
        originalGravityScale = rb.gravityScale; // 保存原始的重力比例
        rb.gravityScale = 0;// 将重力比例设置为 0，禁用重力
    }

    public override void Update()
    {
        base.Update();
        CancelDashIfNeeded();// 检查是否需要取消冲刺并切换状态
        player.SetVelocity(player.dashSpeed * dashDir,0);// 设置玩家的冲刺速度，垂直速度保持 0

        if (stateTimer < 0)// 如果冲刺持续时间结束，根据是否接触地面切换状态
        {
            if(player.groundDetected) // 接触地面，切换到站立状态
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.fallState);// 否则切换到下落状态
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        // 重置玩家速度和恢复重力
        player.SetVelocity(0,0);// 将玩家的速度设置为 0，停止移动
        rb.gravityScale = originalGravityScale;// 恢复原始的重力比例
    }

    // 检查是否碰到墙壁或地面，决定是否取消冲刺并切换状态
    private void CancelDashIfNeeded()
    {
        if (player.wallDetected) // 如果检测到墙壁接触
        {
            if(player.groundDetected)// 如果同时接触地面，切换到站立状态
                stateMachine.ChangeState(player.idleState);
            else// 否则切换到墙壁滑行状态
                stateMachine.ChangeState(player.wallSlideState);
        }
    }
}
