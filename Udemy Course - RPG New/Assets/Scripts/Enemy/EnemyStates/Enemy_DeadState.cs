using UnityEngine;

public class Enemy_DeadState : EnemyState
{
    private Collider2D col;
    
    public Enemy_DeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        col = enemy.GetComponent<Collider2D>();
    }

    public override void Enter()
    {
        anim.enabled = false;// 禁用动画组件，避免在进入该状态时播放动画
        col.enabled = false;// 禁用碰撞体组件，避免与其他物体发生碰撞

        rb.gravityScale = 12;// 修改刚体的重力比例，增加重力影响，使角色下落更快
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 15);// 设置刚体的垂直速度，确保角色朝上或朝下的初始速度
        
        stateMachine.SwitchOffStateMachine();// 关闭当前状态机，停止当前状态下的状态切换
    }
}
