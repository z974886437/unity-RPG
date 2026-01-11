using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    //public StateMachine stateMachine { get; private set; }

    public event Action OnFlipped;

    public Animator anim { get; private set; }//动画师
    public Rigidbody2D rb { get; private set; }//二维刚体
    public Entity_Stats stats { get; private set; }
    protected StateMachine stateMachine;//状态机
    
    private bool facingRight = true;//面向右
    public int facingDir { get; private set; } = 1;//面向方向
    
    [Header("Collision detection")] 
    public LayerMask whatIsGround;//什么是地面
    [SerializeField] private float groundCheckDistance;//地面检查距离
    [SerializeField] private float wallCheckDistance;//墙壁检查距离
    [SerializeField] private Transform groundCheck;//地面检查
    [SerializeField] private Transform primaryWallCheck;//初级墙检查
    [SerializeField] private Transform secondaryWallCheck;//次墙检查
    
    public bool groundDetected { get; private set; }//检测到地面
    public bool wallDetected { get; private set; }//检测到墙壁

    private bool isKnocked;
    private Coroutine knockbackCo;//击退
    private Coroutine slowDownCo;//放慢速度

    // 在对象初始化时调用，进行必要的设置
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<Entity_Stats>();
        
        stateMachine = new StateMachine();// 创建状态机实例
    }

    protected virtual void Start()
    {
        
    }

    // 每帧更新时调用，处理碰撞检测和状态更新
    protected virtual void Update()
    {
        HandleCollisionDetection();// 处理碰撞检测
        stateMachine.UpdateActiveState(); // 更新状态机的当前活动状态
    }
    
    // 调用动画触发器，触发当前状态的动画
    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();// 调用当前状态的动画触发器，通常用于动画事件的触发
    }

    public virtual void EntityDeath()
    {
        
    }

    // 用于使实体减速，启动协程来实现减速效果
    public virtual void SlowDownEntity(float duration,float slowMultiplier,bool canOverrideSlowEffect = false)
    {
        if (slowDownCo != null) // 如果当前已经有减速协程在运行
        {
            if (canOverrideSlowEffect)// 如果允许覆盖已有减速效果
                StopCoroutine(slowDownCo);// 停止当前正在运行的减速协程
            else
                return;// 不允许覆盖时，直接返回，保持原减速效果
        }

        slowDownCo = StartCoroutine(SlowDownEntityCo(duration,slowMultiplier));// 启动新的减速协程，并保存协程引用用于后续管理
    }

    // 实际执行减速效果的协程
    protected virtual IEnumerator SlowDownEntityCo(float duration,float slowMultiplier)
    {
        yield return null;  // 暂时不做任何事情，可以在派生类中重写这个方法实现具体的减速逻辑
    }

    // 停止减速效果的统一出口
    public virtual void StopSlowDown()
    {
        slowDownCo = null;// 清空协程引用，表示当前没有减速效果在运行
    }

    //受到击退
    public void ReciveKnockback(Vector2 knockback, float duration)
    {
        if(knockbackCo != null)// 如果已经有击退协程在运行，停止当前协程
            StopCoroutine(knockbackCo);

        knockbackCo = StartCoroutine(KnockbackCo(knockback, duration));// 启动新的击退协程
    }

    //击退
    private IEnumerator KnockbackCo(Vector2 knockback,float duration)
    {
        isKnocked = true;// 设置被击退标志为 true
        rb.linearVelocity = knockback;// 设置刚体的线性速度为击退的向量，启动击退效果
        
        yield return new WaitForSeconds(duration);// 等待指定的击退持续时间

        rb.linearVelocity = Vector2.zero;// 击退持续时间结束后，将刚体速度重置为零，停止击退
        isKnocked = false;// 设置被击退标志为 false，表示击退结束
    }
    
    

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnocked)
            return;
        
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);// 设置角色的速度，水平速度和垂直速度
        HandleFlip(xVelocity);// 根据水平速度来判断是否需要翻转角色
    }

    public void HandleFlip(float xVelocity)
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
        
        OnFlipped?.Invoke();
    }

    // 进行地面检测，射线从物体当前位置向下发射，检测是否接触地面
    private void HandleCollisionDetection()
    {
        // 射线检测，返回是否与指定的地面层发生碰撞
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        // 如果有第二个墙壁检测点（secondaryWallCheck）
        if (secondaryWallCheck != null)
        {
            // 射线检测，检测从 primaryWallCheck 和 secondaryWallCheck 向右的方向是否与墙壁发生碰撞
            // primaryWallCheck 和 secondaryWallCheck 分别是两个墙壁检测点，Vector2.right * facingDir 是检测的方向（根据朝向，可能是右或左）
            // wallCheckDistance 是射线检测的最大距离，whatIsGround 是墙壁的过滤器
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround)
                           && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        }
        else
            // 如果没有第二个墙壁检测点，则仅使用 primaryWallCheck 进行墙壁检测
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    // 在编辑器中可视化射线，帮助调试地面检测的范围
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position,groundCheck.position + new Vector3(0,- groundCheckDistance)); // 从当前位置绘制向下的射线
        Gizmos.DrawLine(primaryWallCheck.position,primaryWallCheck.position + new Vector3(wallCheckDistance * facingDir,0));
        
        if(secondaryWallCheck != null)// 如果有第二个墙壁检测点（secondaryWallCheck），绘制从 secondaryWallCheck 向右的射线
             Gizmos.DrawLine(secondaryWallCheck.position,secondaryWallCheck.position + new Vector3(wallCheckDistance * facingDir,0));
    }
}
