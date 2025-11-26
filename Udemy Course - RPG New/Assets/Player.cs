using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public StateMachine stateMachine { get; private set; }

    public Animator anim { get; private set; }//动画师
    
    public Rigidbody2D rb { get; private set; }//二维刚体
    
    public PlayerInputSet input { get; private set; }//输入
    
    private StateMachine stateMachine;//状态机
    
    public Player_idleState idleState { get; private set; }//空闲状态
    public Player_MoveState moveState { get; private set; }//移动状态
    public Player_JumpState jumpState { get; private set; }//跳跃状态
    public Player_FallState fallState { get; private set; }//下落状态
    public Player_WallSlideState wallSlideState { get; private set; }//墙体滑动状态
    public Player_WallJumpState wallJumpState { get; private set;}//墙跳状态
    public Player_DashState dashState { get;private set; }//冲刺状态
    public Player_BasicAttackState basicAttackState { get; private set; }//攻击状态

    [Header("Attack details")] 
    public Vector2[] attackVelocity;//攻击速度
    public float attackVelocityDuration = 0.1f;//攻击速度持续时间
    public float comboResetTime = 1;//组合重置时间

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
    
    
    private bool facingRight = true;//面向右
    public int facingDir { get; private set; } = 1;//面向方向

    public Vector2 moveInput { get; private set; }//移动输入

    [Header("Collision detection")] 
    [SerializeField] private float groundCheckDistance;//地面检查距离
    [SerializeField] private float wallCheckDistance;//墙壁检查距离
    [SerializeField] private LayerMask whatIsGround;//什么是地面
    
    public bool groundDetected { get; private set; }//检测到地面
    public bool wallDetected { get; private set; }//检测到墙壁

    // 在对象初始化时调用，进行必要的设置
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        stateMachine = new StateMachine();// 创建状态机实例
        input = new PlayerInputSet();// 创建玩家输入设置实例

        idleState = new Player_idleState(this,stateMachine, "idle");// 创建并初始化玩家空闲状态
        moveState = new Player_MoveState(this,stateMachine, "move");// 创建并初始化玩家移动状态
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new Player_WallJumpState(this,stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "dash");
        basicAttackState = new Player_BasicAttackState(this, stateMachine, "basicAttack");
    }

    // 在对象启用时调用，初始化输入事件
    private void OnEnable()
    {
        input.Enable();// 启用玩家输入系统

        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();// 监听移动输入事件，按下时读取输入值
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;// 监听移动输入取消事件，按下时清空输入值
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    public void CallAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);// 设置角色的速度，水平速度和垂直速度
        HandleFlip(xVelocity);// 根据水平速度来判断是否需要翻转角色
    }

    private void HandleFlip(float xVelocity)
    {
        if(xVelocity > 0 && facingRight == false)// 如果角色向右移动但当前面朝左
            Flip();// 进行翻转
        else if(xVelocity < 0 && facingRight)// 如果角色向左移动但当前面朝右
            Flip();// 进行翻转
    }

    public void Flip()
    {
        transform.Rotate(0,180,0);// 通过旋转角色的 transform 来实现翻转
        facingRight = !facingRight;// 更新角色当前朝向的状态
        facingDir = facingDir * -1;
    }

    // 进行地面检测，射线从物体当前位置向下发射，检测是否接触地面
    private void HandleCollisionDetection()
    {
        // 射线检测，返回是否与指定的地面层发生碰撞
        groundDetected = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    // 在编辑器中可视化射线，帮助调试地面检测的范围
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position,transform.position + new Vector3(0,- groundCheckDistance)); // 从当前位置绘制向下的射线
        Gizmos.DrawLine(transform.position,transform.position + new Vector3(wallCheckDistance * facingDir,0));
    }
}
