using UnityEngine;

public class Player_CounterAttackState : PlayerState
{
    private Player_Combat combat;
    private bool counteredSombody;
    
    public Player_CounterAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        combat = player.GetComponent<Player_Combat>();
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = combat.GetCounterRecoveryDuration();// 获取反击恢复时间
        counteredSombody = combat.CounterAttackPerformed();// 判断是否已执行反击攻击
        
        anim.SetBool("counterAttackPerformed", counteredSombody);// 设置动画的反击标志
    }

    public override void Update()
    {
        base.Update();
        
        player.SetVelocity(0,rb.linearVelocity.y);// 将玩家的水平速度设置为 0，保留垂直速度
        
        if(triggerCalled)// 如果触发条件已满足
            stateMachine.ChangeState(player.idleState);// 切换到闲置状态
        
        if(stateTimer < 0 && counteredSombody == false)// 如果恢复时间已过且没有执行反击
            stateMachine.ChangeState(player.idleState); // 切换到闲置状态
    }
}
