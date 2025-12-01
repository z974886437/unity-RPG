using UnityEngine;

public class Enemy_StunnedState : EnemyState
{
    private Enemy_VFX vfx;
    
    public Enemy_StunnedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        vfx = enemy.GetComponent<Enemy_VFX>();
    }

    public override void Enter()
    {
        base.Enter();

        vfx.EnableAttackAlert(false);// 禁用攻击警报特效
        enemy.EnableCounterWindow(false); // 禁用反击窗口
        
        stateTimer = enemy.stunnedDuration;// 设置状态计时器为敌人的眩晕持续时间
        rb.linearVelocity = new Vector2(enemy.stunnedVelocity.x * -enemy.facingDir, enemy.stunnedVelocity.y);// 设置刚体的线性速度，模拟敌人在眩晕状态下的击退效果
    }

    public override void Update()
    {
        base.Update();
        
        if(stateTimer < 0)// 如果状态计时器小于零，表示眩晕时间结束
            stateMachine.ChangeState(enemy.idleState);// 切换到空闲状态
    }
}
