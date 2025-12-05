using UnityEngine;

public class StateMachine 
{
    
    public EntityState currentState { get; private set; } //当前状态
    public bool canChangeState;//可以改变状态

    // 初始化状态机，设置初始状态并调用进入方法
    public void Initialize(EntityState startState)
    {
        canChangeState = true;
        currentState = startState;// 设置初始状态
        currentState.Enter(); // 执行进入状态操作
    }

    // 切换状态，先退出当前状态再进入新状态
    public void ChangeState(EntityState newState)
    {
        if (canChangeState == false)// 如果当前状态机不允许切换状态，直接返回
            return;
        
        currentState.Exit(); // 退出当前状态
        currentState = newState; // 更新当前状态
        currentState.Enter(); // 执行进入新状态操作
    }

    public void UpdateActiveState()
    {
        currentState.Update();
    }

    public void SwitchOffStateMachine() => canChangeState = false;// 禁止状态机切换状态

}
