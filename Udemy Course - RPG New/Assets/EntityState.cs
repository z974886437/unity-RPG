using UnityEngine;

public abstract class EntityState
{
   protected Player player;
   protected StateMachine stateMachine;//状态机
   protected string animBoolName;

   protected Animator anim;//动画师
   protected Rigidbody2D rb;
   protected PlayerInputSet input;
   
   protected float stateTimer;//状态定时器
   protected bool triggerCalled;//触发呼叫


   public EntityState(Player player,StateMachine stateMachine,string animBoolName)
   {
      this.player = player;// 设置玩家对象
      this.stateMachine = stateMachine;// 设置状态机对象
      this.animBoolName = animBoolName;// 设置动画布尔变量名，用于切换动画状态

      anim = player.anim;// 获取玩家的动画控制器（Animator）
      rb = player.rb;// 获取玩家的刚体（Rigidbody2D）
      input = player.input;// 获取玩家的输入管理器（Input）
   }

   public virtual void Enter()//进入
   {
      // evertime state will be chaned, enter will be called 每次状态改变时，都会调用 Enter
      
      anim.SetBool(animBoolName,true);// 设置动画布尔值为 true，开始相应的动画
      triggerCalled = false;// 重置触发标记，确保新的状态没有立即触发过期的操作
   }

   public virtual void Update()
   {
      // we going to run logic of the state here 我们将在这里运行状态逻辑

      stateTimer -= Time.deltaTime;
      anim.SetFloat("yVelocity",rb.linearVelocity.y);
      
      if(input.Player.Dash.WasPressedThisFrame() && CanDash())// 检查是否按下了冲刺按钮且可以冲刺
         stateMachine.ChangeState(player.dashState);// 如果按下了冲刺按钮且可以冲刺，切换到冲刺状态
   }

   public virtual void Exit()//出口
   {
      // this will be called. everytime we exit state and change to a new one  这将被称为。每次我们退出状态并更改为新状态时
      
      anim.SetBool(animBoolName,false);
   }

   public void CallAnimationTrigger()
   {
      triggerCalled = true;
   }

   // 检查玩家是否能够执行冲刺
   private bool CanDash()
   {
      if (player.wallDetected)// 如果检测到玩家接触到墙壁，不能执行冲刺
         return false;
      
      if(stateMachine.currentState == player.dashState) // 如果当前状态已经是冲刺状态，不能再次执行冲刺
         return false;

      return true;// 如果以上条件都不满足，允许执行冲刺
   }
   
}
