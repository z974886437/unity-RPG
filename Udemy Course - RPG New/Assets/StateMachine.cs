using UnityEngine;

public class StateMachine 
{
    
    public EntityState currentState { get; private set; } //当前状态

    // 初始化状态机，设置初始状态并调用进入方法
    public void Initialize(EntityState startState)
    {
        currentState = startState;// 设置初始状态
        currentState.Enter(); // 执行进入状态操作
    }

    // 切换状态，先退出当前状态再进入新状态
    public void ChangeState(EntityState newState) 
    {
        currentState.Exit(); // 退出当前状态
        currentState = newState; // 更新当前状态
        currentState.Enter(); // 执行进入新状态操作
    }
    
}
