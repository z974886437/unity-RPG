using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player_SwordThrowState : PlayerState
{
    private Camera mainCamera;
    
    public Player_SwordThrowState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    // 状态机的进入方法，初始化主摄像头
    public override void Enter()
    {
        base.Enter();
        
        skillManager.swordThrow.EnableDots(true);
        
        // 如果 mainCamera 不是主摄像头，设置为主摄像头
        if(mainCamera != Camera.main)
            mainCamera = Camera.main;
    }

    // 更新方法，处理玩家输入和动画
    public override void Update()
    {
        base.Update();

        Vector2 dirToMouse = DirectionToMouse();// 计算玩家朝向鼠标的方向
        
        player.SetVelocity(0,rb.linearVelocity.y);// 设置玩家的速度，保持垂直速度不变
        player.HandleFlip(dirToMouse.x);// 处理玩家角色的翻转（朝向）
        skillManager.swordThrow.PredictTrajectory(dirToMouse);

        if (input.Player.Attack.WasPressedThisFrame())// 如果按下攻击按钮，设置动画参数触发剑投掷
        {
            anim.SetBool("swordThrowPerformed", true);
            
            skillManager.swordThrow.EnableDots(false);
            skillManager.swordThrow.ConfirmTrajectory(dirToMouse);

        }
        
        if(input.Player.RangeAttack.WasReleasedThisFrame() || triggerCalled) // 如果释放远程攻击按钮或触发器被调用，切换到空闲状态
            stateMachine.ChangeState(player.idleState);
    }

    // 状态机退出方法，重置动画参数
    public override void Exit()
    {
        base.Exit();
        
        anim.SetBool("swordThrowPerformed", false);// 重置剑投掷的动画参数
        skillManager.swordThrow.EnableDots(false);
    }

    // 计算玩家朝向鼠标的方向
    private Vector2 DirectionToMouse()
    {
        Vector2 playerPosition = player.transform.position;// 获取玩家位置
        Vector2 worldMousePosition = mainCamera.ScreenToWorldPoint(player.mousePosition);// 将鼠标位置从屏幕坐标转换为世界坐标

        Vector2 direction = worldMousePosition - playerPosition;// 计算从玩家到鼠标的方向并返回单位向量

        return direction.normalized;
    }
}
