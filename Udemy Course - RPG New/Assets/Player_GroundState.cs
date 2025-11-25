using UnityEngine;

public class Player_GroundSedSt : EntityState
{
    public Player_GroundSedSt(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();
        
        if(input.Player.Jump.WasPerformedThisFrame())// 检查玩家是否在当前帧内按下跳跃按钮
            stateMachine.ChangeState(player.jumpState);// 如果跳跃按钮被按下，切换到跳跃状态
    }
}
