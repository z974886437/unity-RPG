using UnityEngine;

public abstract class EntityState
{
    protected StateMachine stateMachine;//状态机
    protected string animBoolName;

    protected Animator anim;//动画师
    protected Rigidbody2D rb;
   
    protected float stateTimer;//状态定时器
    protected bool triggerCalled;//触发呼叫

    public EntityState(StateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;// 设置状态机对象
        this.animBoolName = animBoolName;// 设置动画布尔变量名，用于切换动画状态
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
        UpdateAnimationParameters();
    }

    public virtual void Exit()//出口
    {
        // this will be called. everytime we exit state and change to a new one  这将被称为。每次我们退出状态并更改为新状态时
      
        anim.SetBool(animBoolName,false);
    }

    public void AnimationTrigger()
    {
        triggerCalled = true;
    }

    public virtual void UpdateAnimationParameters()
    {
        
    }
}
