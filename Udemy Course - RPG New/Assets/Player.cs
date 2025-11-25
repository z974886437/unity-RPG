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


    [Header("Movement details")] 
    public float moveSpeed;//移动速度
    public float jumpForce = 5;//跳跃力
    private bool facingRight = true;//面向右

    public Vector2 moveInput { get; private set; }//移动输入

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
    }

    // 在对象启用时调用，初始化输入事件
    private void OnEnable()
    {
        input.Enable();// 启用玩家输入系统

        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();// 监听移动输入事件，按下时读取输入值
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;// 监听移动输入取消事件，按下时清空输入值

       ;
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
        stateMachine.UpdateActiveState();
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

    private void Flip()
    {
        transform.Rotate(0,180,0);// 通过旋转角色的 transform 来实现翻转
        facingRight = !facingRight;// 更新角色当前朝向的状态
    }
}
