using UnityEngine;

public class Enemy_MoveState : Enemy_GroundedState
{
    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        if (enemy.groundDetected == false || enemy.wallDetected)// 如果敌人未检测到地面或检测到墙壁，则调用敌人的 Flip 方法，改变朝向
            enemy.Flip();
    }

    public override void Update()
    {
        base.Update();
        
        enemy.SetVelocity(enemy.GetMoveSpeed() * enemy.facingDir,rb.linearVelocity.y); // 设置敌人的速度，根据朝向和速度设置水平方向的速度

        if (enemy.groundDetected == false || enemy.wallDetected)// 如果敌人未检测到地面或检测到墙壁，切换到空闲状态
            stateMachine.ChangeState(enemy.idleState);
    }
}
