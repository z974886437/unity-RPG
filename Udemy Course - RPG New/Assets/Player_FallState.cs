using UnityEngine;

public class Player_FallState : Player_AireState
{
    public Player_FallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();
        
        
        if(player.groundDetected)// 如果检测到玩家接触地面，切换到站立状态
            stateMachine.ChangeState(player.idleState); // 切换到站立（Idle）状态
    }
}
