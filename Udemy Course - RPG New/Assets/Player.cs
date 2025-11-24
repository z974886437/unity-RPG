using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public StateMachine stateMachine { get; private set; }

    public Animator anim { get; private set; }
    
    private PlayerInputSet input;//输入
    private StateMachine stateMachine;//状态机
    
    public Player_idleState idleState { get; private set; }//空闲状态
    public Player_MoveState moveState { get; private set; }//移动状态

    public Vector2 moveInput { get; private set; }//移动输入

    // 在对象初始化时调用，进行必要的设置
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        
        stateMachine = new StateMachine();// 创建状态机实例
        input = new PlayerInputSet();// 创建玩家输入设置实例

        idleState = new Player_idleState(this,stateMachine, "idle");// 创建并初始化玩家空闲状态
        moveState = new Player_MoveState(this,stateMachine, "move");// 创建并初始化玩家移动状态
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
        stateMachine.UpdateActiveState();
    }
}
