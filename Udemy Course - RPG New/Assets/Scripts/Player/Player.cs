using System;
using System.Collections;
using UnityEngine;

public class Player : Entity
{
    public static event Action OnPlayerDeath;
    private UI ui;
    public PlayerInputSet input { get; private set; }//输入
    public Player_SkillManager skillManager { get; private set; }
    public Player_VFX vfx { get; private set; }
    public Entity_Health health { get; private set; }
    public Entity_StatusHandler statusHandler { get; private set; }
    
    #region State Variables
    
    public Player_IdleState idleState { get; private set; }//空闲状态
    public Player_MoveState moveState { get; private set; }//移动状态
    public Player_JumpState jumpState { get; private set; }//跳跃状态
    public Player_FallState fallState { get; private set; }//下落状态
    public Player_WallSlideState wallSlideState { get; private set; }//墙体滑动状态
    public Player_WallJumpState wallJumpState { get; private set;}//墙跳状态
    public Player_DashState dashState { get;private set; }//冲刺状态
    public Player_BasicAttackState basicAttackState { get; private set; }//攻击状态
    public Player_JumpAttackState jumpAttackState { get; private set; }//跳跃攻击状态
    public Player_DeadState deadState { get; private set; }
    public Player_CounterAttackState counterAttackState { get; private set; }
    public Player_SwordThrowState swordThrowState { get; private set; }
    
    #endregion
    
    [Header("Attack details")] 
    public Vector2[] attackVelocity;//攻击速度
    public Vector2 jumpAttackVelocity;//跳跃攻击速度
    public float attackVelocityDuration = 0.1f;//攻击速度持续时间
    public float comboResetTime = 1;//组合重置时间
    private Coroutine queuedAttackCo;//排队攻击协程

    [Header("Movement details")] 
    public float moveSpeed;//移动速度
    public float jumpForce = 5;//跳跃力
    public Vector2 wallJumpForce;//墙跳方向
    [Range(0,1)]
    public float inAirMoveMultiplier = 0.7f;//空中移动乘数
    [Range(0,1)]
    public float wallSlideSlowMultiplier = 0.7f;//墙壁滑梯慢速倍增器
    [Space] 
    public float dashDuration = 0.25f;//冲刺持续时间
    public float dashSpeed = 20;//冲刺速度
    
    public Vector2 moveInput { get; private set; }//移动输入
    public Vector2 mousePosition { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
        ui = FindAnyObjectByType<UI>();
        skillManager = GetComponent<Player_SkillManager>();
        statusHandler = GetComponent<Entity_StatusHandler>();
        vfx = GetComponent<Player_VFX>();
        health = GetComponent<Entity_Health>();

        input = new PlayerInputSet();// 创建玩家输入设置实例
        
        idleState = new Player_IdleState(this,stateMachine, "idle");// 创建并初始化玩家空闲状态
        moveState = new Player_MoveState(this,stateMachine, "move");// 创建并初始化玩家移动状态
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new Player_WallJumpState(this,stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "dash");
        basicAttackState = new Player_BasicAttackState(this, stateMachine, "basicAttack");
        jumpAttackState = new Player_JumpAttackState(this, stateMachine, "jumpAttack");
        deadState = new Player_DeadState(this, stateMachine, "dead");
        counterAttackState = new Player_CounterAttackState(this, stateMachine, "counterAttack");
        swordThrowState = new Player_SwordThrowState(this,stateMachine,"swordThrow");
    }
    
    // 游戏对象开始时的初始化
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);// 初始化状态机，设置初始状态为 idleState（空闲状态）
    }

    // 传送玩家到指定位置
    public void TeleportPlayer(Vector3 position) => transform.position = position;// 将玩家位置设置为目标位置

    // 实现实体减速的协程，调整实体的多项属性，如移动速度、跳跃力、动画速度等
    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        float originalMoveSpeed = moveSpeed;// 原始的移动速度
        float originalJumpForce = jumpForce; // 原始的跳跃力
        float originalAnimSpeed = anim.speed; // 原始的动画速度
        Vector2 originalWallJump = wallJumpForce; // 原始的墙跳力
        Vector2 originalJumpAttack = jumpAttackVelocity; // 原始的跳跃攻击速度
        Vector2[] originalAttackVelocity = attackVelocity; // 原始的攻击速度（向量数组）

        float speedMultiplier = 1 - slowMultiplier;// 根据减速倍率计算新的速度倍率

        moveSpeed = moveSpeed * speedMultiplier; // 改变移动速度
        jumpForce = jumpForce * speedMultiplier; // 改变跳跃力
        anim.speed = anim.speed * speedMultiplier; // 改变动画速度
        wallJumpForce = wallJumpForce * speedMultiplier; // 改变墙跳力
        jumpAttackVelocity = jumpAttackVelocity * speedMultiplier; // 改变跳跃攻击速度

        for (int i = 0; i < attackVelocity.Length; i++)// 对攻击速度数组中的每个值应用减速效果
        {
            attackVelocity[i] = attackVelocity[i] * speedMultiplier; // 对每个攻击速度进行调整
        }

        yield return new WaitForSeconds(duration);// 等待减速持续时间

        moveSpeed = originalMoveSpeed; // 恢复原始的移动速度
        jumpForce = originalJumpForce; // 恢复原始的跳跃力
        anim.speed = originalAnimSpeed; // 恢复原始的动画速度
        wallJumpForce = originalWallJump; // 恢复原始的墙跳力
        jumpAttackVelocity = originalJumpAttack; // 恢复原始的跳跃攻击速度

        for (int i = 0; i < attackVelocity.Length; i++) // 恢复攻击速度数组中的每个值
        {
            attackVelocity[i] = originalAttackVelocity[i]; // 恢复每个攻击速度
        }
    }

    public override void EntityDeath()
    {
        base.EntityDeath();
        
        OnPlayerDeath?.Invoke(); // 如果有订阅者，触发玩家死亡事件（例如 UI 更新、音效播放等）
        stateMachine.ChangeState(deadState);// 切换到死亡状态，处理死亡后的状态逻辑
    }

    // 延迟进入攻击状态
    public void EnterAttackStateWithDelay()
    {
        if(queuedAttackCo != null) // 如果已经有一个排队的攻击协程在运行，停止它
            StopCoroutine(queuedAttackCo);

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());// 启动一个新的协程，延迟进入攻击状态
    }

    // 协程：延迟进入攻击状态
    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();// 等待当前帧结束后再执行状态切换
        stateMachine.ChangeState(basicAttackState);// 切换到基本攻击状态
    }
    
    // 在对象启用时调用，初始化输入事件
    private void OnEnable()
    {
        input.Enable();// 启用玩家输入系统
        
        input.Player.Mouse.performed += ctx => mousePosition = ctx.ReadValue<Vector2>();

        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();// 监听移动输入事件，按下时读取输入值
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;// 监听移动输入取消事件，按下时清空输入值

        input.Player.ToggleSkillTreeUI.performed += ctx => ui.ToggleSkillTreeUI();// 监听切换技能树UI的输入事件
        
        input.Player.Spell.performed += ctx => skillManager.shard.TryUseSkill();// 监听施放技能的输入事件
        input.Player.Spell.performed += ctx => skillManager.timeEcho.TryUseSkill();// 监听施放技能的输入事件
    }
    
    // 当对象禁用时调用，通常用于释放资源
    private void OnDisable()
    {
        input.Disable();// 禁用输入系统，停止监听输入
    }
    
    
}
