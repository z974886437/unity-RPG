using UnityEngine;

public class Player_WallJumpState : PlayerState
{
    public Player_WallJumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        player.SetVelocity(player.wallJumpForce.x * -player.facingDir,player.wallJumpForce.y);// 设置墙壁跳跃的速度，水平速度根据角色的朝向，垂直速度设为墙壁跳跃的垂直力
    }

    public override void Update()
    {
        base.Update();
        
        if(rb.linearVelocity.y < 0)// 如果玩家的垂直速度小于 0，说明正在下落，切换到下落状态
            stateMachine.ChangeState(player.fallState);
        
        if(player.wallDetected) // 如果检测到墙壁接触，切换到墙壁滑行状态
            stateMachine.ChangeState(player.wallSlideState);
    }
}
