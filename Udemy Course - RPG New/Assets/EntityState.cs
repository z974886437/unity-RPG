using UnityEngine;

public abstract class EntityState
{
   protected Player player;
   protected StateMachine stateMachine;//状态机
   protected string animBoolName;

   protected Animator anim;//动画师
   protected Rigidbody2D rb;
   protected PlayerInputSet input;


   public EntityState(Player player,StateMachine stateMachine,string animBoolName)
   {
      this.player = player;
      this.stateMachine = stateMachine;
      this.animBoolName = animBoolName;

      anim = player.anim;
      rb = player.rb;
      input = player.input;
   }

   public virtual void Enter()//进入
   {
      // evertime state will be chaned, enter will be called 每次状态改变时，都会调用 Enter
      
      anim.SetBool(animBoolName,true);
   }

   public virtual void Update()
   {
      // we going to run logic of the state here 我们将在这里运行状态逻辑
      
      anim.SetFloat("yVelocity",rb.linearVelocity.y);
   }

   public virtual void Exit()//出口
   {
      // this will be called. everytime we exit state and change to a new one  这将被称为。每次我们退出状态并更改为新状态时
      
      anim.SetBool(animBoolName,false);
   }
   
}
