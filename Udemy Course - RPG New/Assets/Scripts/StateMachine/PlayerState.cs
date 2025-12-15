using UnityEngine;

public abstract class PlayerState : EntityState
{
   protected Player player;
   protected PlayerInputSet input;
   protected Player_SkillManager skillManager;

   public PlayerState(Player player,StateMachine stateMachine,string animBoolName) : base(stateMachine,animBoolName)
   {
      this.player = player;// 设置玩家对象

      anim = player.anim;// 获取玩家的动画控制器（Animator）
      rb = player.rb;// 获取玩家的刚体（Rigidbody2D）
      input = player.input;// 获取玩家的输入管理器（Input）
      stats = player.stats;
      skillManager = player.skillManager;
   }

   public override void Update()
   {
      base.Update();
      
      if(input.Player.Dash.WasPressedThisFrame() && CanDash())// 检查是否按下了冲刺按钮且可以冲刺
      {
         skillManager.dash.SetSkillOnCooldown();
         stateMachine.ChangeState(player.dashState);// 如果按下了冲刺按钮且可以冲刺，切换到冲刺状态
      }
   }

   public override void UpdateAnimationParameters()
   {
      base.UpdateAnimationParameters();
      
      anim.SetFloat("yVelocity",rb.linearVelocity.y);
   }

   // 检查玩家是否能够执行冲刺
   private bool CanDash()
   {
      if(skillManager.dash.CanUseSkill() == false)
         return false;
      
      if (player.wallDetected)// 如果检测到玩家接触到墙壁，不能执行冲刺
         return false;
      
      if(stateMachine.currentState == player.dashState) // 如果当前状态已经是冲刺状态，不能再次执行冲刺
         return false;

      return true;// 如果以上条件都不满足，允许执行冲刺
   }
   
}
