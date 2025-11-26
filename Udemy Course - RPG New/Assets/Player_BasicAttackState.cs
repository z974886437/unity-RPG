using UnityEngine;

public class Player_BasicAttackState : EntityState
{
    private float attackVelocityTimer;//攻击速度计时器
    
    private const int FirstComboIndex = 1;// we start combo index wit number 1,this parametr is used in the Animator.
    private int comboIndex = 1;//组合指数
    private int comboLimit = 3;//组合限制

    private float lastTimeAttacked;//上次攻击
    
    
    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (comboLimit != player.attackVelocity.Length)// 检查攻击组合限制是否与攻击速度数组长度一致，如果不一致，则调整
        {
            Debug.LogWarning("I've adjusted combo limit,according to attack velocity array!");
            comboLimit = player.attackVelocity.Length;
        }
    }

    public override void Enter()
    {
        base.Enter();

        ResetComboIndexIfNeeded();// 检查是否需要重置组合索引
        
        anim.SetInteger("baiscAttackIndex",comboIndex);// 设置当前攻击索引的动画参数
        ApplyAttackVelocity();// 生成并应用攻击时的速度
    }

    

    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();// 处理攻击时的速度
        
        if(triggerCalled)// 如果触发条件已调用，切换回站立状态
            stateMachine.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        base.Exit();
        comboIndex++;// 组合索引自增
        lastTimeAttacked = Time.time;// 更新上次攻击时间
    }

    // 处理攻击时的速度，倒计时结束后恢复玩家的垂直速度
    private void HandleAttackVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;// 递减攻击速度计时器（每秒减少时间）
        
        if(attackVelocityTimer < 0)// 如果攻击时间结束，将玩家的水平速度设为 0，保持垂直速度
            player.SetVelocity(0,rb.linearVelocity.y);// 恢复垂直速度，水平速度设为 0
    }

    // 生成攻击时的速度，初始化计时器并设置玩家的攻击速度
    private void ApplyAttackVelocity()
    {
        Vector2 attackVelocity = player.attackVelocity[comboIndex - 1];
        
        attackVelocityTimer = player.attackVelocityDuration;// 设置攻击速度持续时间
        player.SetVelocity(attackVelocity.x * player.facingDir,attackVelocity.y);// 设置攻击时的速度，水平速度由攻击方向控制
    }
    
    // 如果需要，重置组合索引
    private void ResetComboIndexIfNeeded()
    {
        if (Time.time > lastTimeAttacked + player.comboResetTime)// 如果上次攻击后已超过重置时间，重置组合索引为初始值
            comboIndex = FirstComboIndex;
        
        if (comboIndex > comboLimit)// 如果组合索引超过限制，重置为初始值
            comboIndex = FirstComboIndex;
    }
}
