using UnityEngine;

public class Player_GroundedState : PlayerState
{
    public Player_GroundedState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();
        
        if(rb.linearVelocity.y < 0 && player.groundDetected == false)// 如果玩家的垂直速度小于 0，说明玩家正在下落，切换到下落状态
            stateMachine.ChangeState(player.fallState);
        
        if(input.Player.Jump.WasPressedThisFrame())// 检查玩家是否在当前帧内按下跳跃按钮
            stateMachine.ChangeState(player.jumpState);// 如果跳跃按钮被按下，切换到跳跃状态
        
        if(input.Player.Attack.WasPressedThisFrame()) // 如果玩家在当前帧内按下攻击按钮，切换到基本攻击状态
            stateMachine.ChangeState(player.basicAttackState);// 切换到基本攻击（BasicAttack）状态
        
        if(input.Player.CounterAttack.WasPressedThisFrame())// 检查玩家是否按下反击按钮，如果按下，则切换到反击状态
            stateMachine.ChangeState(player.counterAttackState);
        
        if(input.Player.RangeAttack.WasPressedThisFrame() && skillManager.swordThrow.CanUseSkill())// 检查玩家是否按下远程攻击按钮，如果按下，则切换到投剑状态
            stateMachine.ChangeState(player.swordThrowState);
    }
}
