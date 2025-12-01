using UnityEngine;

public class Enemy_IdleState : Enemy_GroundedState
{
    public Enemy_IdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;// 设置状态计时器，敌人的空闲时间（例如，敌人在空闲状态下会等待一段时间后移动）
    }

    public override void Update()
    {
        base.Update();
        
        if(stateTimer < 0 )// 如果状态计时器小于 0，表示空闲时间已经结束，切换到移动状态
            stateMachine.ChangeState(enemy.moveState);// 切换到敌人的移动状态
    }
}
