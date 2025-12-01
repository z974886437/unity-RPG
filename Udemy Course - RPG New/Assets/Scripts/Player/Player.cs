using System;
using System.Collections;
using UnityEngine;

public class Player : Entity
{
    public static event Action OnPlayerDeath;
    public PlayerInputSet input { get; private set; }//输入
    
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

    protected override void Awake()
    {
        base.Awake();
        
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
    }
    
    // 游戏对象开始时的初始化
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);// 初始化状态机，设置初始状态为 idleState（空闲状态）
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

        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();// 监听移动输入事件，按下时读取输入值
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;// 监听移动输入取消事件，按下时清空输入值
    }
    
    // 当对象禁用时调用，通常用于释放资源
    private void OnDisable()
    {
        input.Disable();// 禁用输入系统，停止监听输入
    }
    
    
}
