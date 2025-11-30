using UnityEngine;

public class Player_BasicAttackState : PlayerState
{
    private float attackVelocityTimer;//攻击速度计时器
    private float lastTimeAttacked;//上次攻击
    
    private bool comboAttackQueued;//组合攻击已排队
    private int attackDir;//攻击方向
    private int comboIndex = 1;//组合指数
    private int comboLimit = 3;//组合限制
    private const int FirstComboIndex = 1;// we start combo index wit number 1,this parametr is used in the Animator.

    
    
    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (comboLimit != player.attackVelocity.Length)// 检查攻击组合限制是否与攻击速度数组长度一致，如果不一致，则调整
        {
            Debug.LogWarning("adjusted combo limit to match  attack velocity array!");
            comboLimit = player.attackVelocity.Length;// 调整组合限制为攻击速度数组的长度
        }
    }

    public override void Enter()
    {
        base.Enter();
        comboAttackQueued = false;
        ResetComboIndexIfNeeded();// 检查是否需要重置组合索引

        // if (player.moveInput.x != 0)
        //     attackDir = ((int)player.moveInput.x);
        // else
        //     attackDir = player.facingDir;
        // 根据玩家的输入方向改变攻击方向
        // 如果玩家有水平输入，则攻击方向跟随输入的x轴方向
        // 否则攻击方向与玩家的朝向一致
        attackDir = player.moveInput.x != 0 ? ((int)player.moveInput.x) : player.facingDir;
        
        anim.SetInteger("baiscAttackIndex",comboIndex);// 设置当前攻击索引的动画参数
        ApplyAttackVelocity();// 生成并应用攻击时的速度
    }

    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();// 处理攻击时的速度

        if (input.Player.Attack.WasPressedThisFrame())// 检查玩家是否按下了攻击按钮，如果是，则排队下一个攻击
            QueueNextAttack();// 排队下一个攻击，准备执行组合攻击

        if (triggerCalled) // 如果触发条件已调用，切换回站立状态
            HandleStateExit();
    }

    public override void Exit()
    {
        base.Exit();
        comboIndex++;// 组合索引自增
        lastTimeAttacked = Time.time;// 更新上次攻击时间
    }

    // 处理状态退出时的逻辑
    private void HandleStateExit()
    {
        if (comboAttackQueued)// 如果下一个攻击已经排队
        {
            anim.SetBool(animBoolName,false);// 结束当前攻击动画
            player.EnterAttackStateWithDelay(); // 延迟进入下一个攻击状态
        }
        else
            stateMachine.ChangeState(player.idleState);// 否则，切换到站立（空闲）状态
    }

    // 排队下一个攻击
    private void QueueNextAttack()
    {
        if (comboIndex < comboLimit)// 如果当前攻击组合索引小于最大组合限制
            comboAttackQueued = true; // 标记当前攻击已经排队
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
        Vector2 attackVelocity = player.attackVelocity[comboIndex - 1]; // 获取当前组合攻击的速度（根据组合索引）
        
        attackVelocityTimer = player.attackVelocityDuration;// 设置攻击速度持续时间
        player.SetVelocity(attackVelocity.x * attackDir,attackVelocity.y);// 设置玩家的速度，水平速度根据攻击方向控制，垂直速度直接使用攻击的 y 值
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
