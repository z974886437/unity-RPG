using UnityEngine;

public class Player_DomainExpansionState : PlayerState
{
    private Vector2 originalPosition;//原来的位置
    private float originalGravity;//原始重力
    private float maxDistanceToGoUp;//最终上升距离

    private bool isLevitating;//正在悬浮
    private bool createdDomain;//创建的域
    
    public Player_DomainExpansionState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    // 进入上升状态时调用，用于初始化上升所需的所有数据
    public override void Enter()
    {
        base.Enter();
        
        originalPosition = player.transform.position;// 记录进入状态时角色的初始位置，用于后续计算上升距离
        originalGravity = rb.gravityScale;// 记录原本的重力值，退出状态时需要还原
        maxDistanceToGoUp = GetAvailableRiseDistance();// 根据头顶空间计算本次最多可以上升的距离，防止穿模
        
        player.SetVelocity(0,player.riseSpeed);// 给角色一个向上的初速度，开始上升
    }

    // 每一帧更新状态逻辑
    public override void Update()
    {
        base.Update();// 调用父类 Update，保证计时器等基础逻辑运行
        
        // 如果上升距离达到上限且还没进入悬浮状态，则切换为悬浮
        if(Vector2.Distance(originalPosition,player.transform.position) >= maxDistanceToGoUp && isLevitating == false)
            Levitate();

        if (isLevitating)// 如果已经处于悬浮状态
        {
            if (stateTimer <= 0)// 当悬浮计时结束后，切回 Idle 状态
                stateMachine.ChangeState(player.idleState);
        }
    }

    // 离开该状态时调用，用于清理和还原数据
    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = originalGravity;// 恢复进入状态前的重力，避免角色永久失重
        isLevitating = false;// 重置悬浮标记，为下次进入状态做准备
        createdDomain = false;// 重置领域创建标记，允许下次重新生成
    }

    // 进入悬浮状态的具体处理逻辑
    private void Levitate()
    {
        isLevitating = true;// 标记角色已进入悬浮状态
        rb.linearVelocity = Vector2.zero;// 清空速度，防止角色继续移动
        rb.gravityScale = 0; // 关闭重力，实现悬浮效果

        stateTimer = 2;// 设置悬浮持续时间（秒）

        if (createdDomain == false) // 如果还没有创建领域
        {
            createdDomain = true;// 标记领域已创建，防止重复生成
            skillManager.domainExpansion.CreateDomain();// 调用技能系统生成领域效果
        }
    }

    // 计算角色头顶还能上升多少距离
    private float GetAvailableRiseDistance()
    {
        // 从角色当前位置向上发射射线，检测是否有地面或天花板
        RaycastHit2D hit =
            Physics2D.Raycast(player.transform.position, Vector2.up, player.riseMaxDistance, player.whatIsGround);

        // 如果射线检测到碰撞体，则返回安全距离（留 1 单位缓冲），否则返回最大上升距离
        return hit.collider != null ? hit.distance - 1 : player.riseMaxDistance;
    }
}
