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

   // 每一帧处理玩家在 Idle（或通用）状态下的输入逻辑
   public override void Update()
   {
      base.Update();
      
      if(input.Player.Dash.WasPressedThisFrame() && CanDash())// 检查是否按下了冲刺按钮且可以冲刺
      {
         skillManager.dash.SetSkillOnCooldown();// 触发冲刺技能冷却，防止短时间内重复使用
         stateMachine.ChangeState(player.dashState);// 如果按下了冲刺按钮且可以冲刺，切换到冲刺状态
      }

      // 检测当前帧是否按下终极技能键，并且领域技能满足使用条件
      if (input.Player.UltimateSpell.WasPressedThisFrame() && skillManager.domainExpansion.CanUseSkill())
      {
         if (skillManager.domainExpansion.InstantDomain())// 如果该领域升级支持“瞬发”，则直接生成领域
         {
            skillManager.domainExpansion.CreateDomain();// 立刻创建领域技能对象，不经过额外状态
         }
         else
         {
            stateMachine.ChangeState(player.domainExpansionState);// 否则切换到领域展开状态，由状态逻辑控制生成流程
         }
         
         skillManager.domainExpansion.SetSkillOnCooldown();// 无论是否瞬发，都立即进入冷却，保证技能使用规则一致
      }
   }

   // 每一帧向 Animator 同步当前状态所需的动画参数
   public override void UpdateAnimationParameters()
   {
      base.UpdateAnimationParameters();
      
      anim.SetFloat("yVelocity",rb.linearVelocity.y); // 将刚体当前的垂直速度传给 Animator，用于区分上升、下落或悬浮动画
   }

   // 检查玩家是否能够执行冲刺
   private bool CanDash()
   {
      if(skillManager.dash.CanUseSkill() == false)
         return false;
      
      if (player.wallDetected)// 如果检测到玩家接触到墙壁，不能执行冲刺
         return false;
      
      if(stateMachine.currentState == player.dashState || stateMachine.currentState == player.domainExpansionState) // 如果当前状态已经是冲刺状态，不能再次执行冲刺
         return false;

      return true;// 如果以上条件都不满足，允许执行冲刺
   }
   
}
