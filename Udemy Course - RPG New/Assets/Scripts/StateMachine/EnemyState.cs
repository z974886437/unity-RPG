using UnityEngine;

public class EnemyState : EntityState
{
    protected Enemy enemy;
    
    public EnemyState(Enemy enemy,StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.enemy = enemy;

        rb = enemy.rb;
        anim = enemy.anim;
        stats = enemy.stats;
    }

    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();
        
        float battleAnimSpeedMultiplier = enemy.battleMoveSpeed / enemy.moveSpeed;
        
        anim.SetFloat("battleAnimSpeedMultiplier",battleAnimSpeedMultiplier);
        anim.SetFloat("moveAnimSpeedMultiplier",enemy.moveAnimSpeedMultiplier);// 设置动画控制器中的 "moveAnimSpeedMultiplier" 参数，用于调整敌人移动动画的速度
        anim.SetFloat("xVelocity",rb.linearVelocity.x);
    }
}
