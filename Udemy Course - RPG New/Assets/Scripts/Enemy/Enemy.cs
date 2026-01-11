using System;
using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;//空闲状态
    public Enemy_MoveState moveState;//移动状态
    public Enemy_AttackState attackState;//攻击状态
    public Enemy_BattleState battleState;//战斗状态
    public Enemy_DeadState deadState;//死亡状态
    public Enemy_StunnedState stunnedState;//惊呆状态
    
    [Header("Battle details")]
    public float battleMoveSpeed = 3;//战斗移动速度
    public float attackDistance = 2;//攻击距离
    public float battleTimeDuration = 5;//战斗持续时间
    public float minRetreatDistance = 1;//最小后退距离
    public Vector2 retrealVelocity;//后退速度
    
    [Header("Stunned state details")]
    public float stunnedDuration = 1;
    public Vector2 stunnedVelocity = new Vector2(7,7);
    protected bool canBeStunned;
    
    [Header("Movement details")] 
    public float idleTime = 2;//空闲时间
    public float moveSpeed = 1.4f;//移动速度
    [Range(0,2)]
    public float moveAnimSpeedMultiplier = 1;//移动动画速度倍增器
    
    [Header("Player detection")]
    [SerializeField] private LayerMask whatIsPlayer;//什么是玩家
    [SerializeField] private Transform playerCheck;//玩家检测
    [SerializeField] private float playerCheckDistance = 10;//检测玩家距离
    
    public Transform player { get; private set; }
    public float activeSlowMultiplier { get; private set; } = 1;//主动慢速乘法器

    
    // 获取当前真实移动速度：基础移动速度 × 当前减速倍率
    public float GetMoveSpeed() => moveSpeed * activeSlowMultiplier;
    
    // 获取当前真实战斗移动速度：基础战斗速度 × 当前减速倍率
    public float GetBattleMoveSpeed() => battleMoveSpeed * activeSlowMultiplier;

    // 使实体在指定时间内减速的协程
    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        #region original
        // // 保存原始的移动速度、战斗速度和动画速度
        // float originalMoveSpeed = moveSpeed; // 记录原始的移动速度
        // float originalBattleSpeed = battleMoveSpeed; // 记录原始的战斗速度
        // float originalAnimSpeed = anim.speed; // 记录原始的动画播放速度
        //
        // // 计算实际减速比例（减速倍率控制）
        // float speedMultiplier = 1 - slowMultiplier; // 1 减去减速倍率得到新的速度比例
        //
        // // 应用减速效果，减缓角色的移动速度、战斗速度和动画速度
        // moveSpeed = moveSpeed * speedMultiplier; // 角色的移动速度按减速比例调整
        // battleMoveSpeed = battleMoveSpeed * speedMultiplier; // 角色的战斗移动速度按减速比例调整
        // anim.speed = anim.speed * speedMultiplier; // 角色的动画速度按减速比例调整
        //
        // moveSpeed = originalMoveSpeed; // 恢复原始的移动速度
        // battleMoveSpeed = originalBattleSpeed; // 恢复原始的战斗速度
        // anim.speed = originalAnimSpeed;// 恢复原始的动画速度
        #endregion
        activeSlowMultiplier = 1 - slowMultiplier;// 根据减速百分比计算最终速度倍率（例如 0.3 表示减速 30%）

        anim.speed = anim.speed * activeSlowMultiplier; // 同步降低动画播放速度，使动作与移动速度一致
        
        yield return new WaitForSeconds(duration); // 等待减速效果的持续时间
        StopSlowDown();// 时间结束后，恢复正常速度
    }

    // 立即停止减速效果，用于时间结束或离开领域
    public override void StopSlowDown()
    {
        activeSlowMultiplier = 1;// 将减速倍率重置为 1，表示不再减速
        anim.speed = 1;// 将动画速度重置为 1，恢复正常播放
        base.StopSlowDown();// 调用父类的停止减速逻辑（用于清理状态或标记）
        
    }

    // 启用或禁用反击窗口（反击窗口决定了是否可以被眩晕或受到控制）
    public void EnableCounterWindow(bool enable) => canBeStunned = enable;

    // 实体死亡时的处理逻辑
    public override void EntityDeath()
    {
        base.EntityDeath();// 调用父类的死亡逻辑（如果有）
        
        stateMachine.ChangeState(deadState); // 通过状态机将实体状态切换到“死亡”状态
    }

    // 处理玩家死亡时的状态切换
    private void HandlePlayerDeath()
    {
        stateMachine.ChangeState(idleState); // 玩家死亡后，切换到“待机”状态
    }

    // 尝试进入战斗状态的方法
    public void TryEnterBattleState(Transform player)
    {
        if (stateMachine.currentState == battleState)// 如果当前状态已经是战斗状态，直接返回
            return;

        if (stateMachine.currentState == attackState)// 如果当前状态是攻击状态，不能进入战斗状态，直接返回
            return;
        
        this.player = player;// 设置当前玩家对象
        stateMachine.ChangeState(battleState);// 切换到战斗状态
    }

    // 获取玩家引用的方法
    public Transform GetPlayerReference()
    {
        if (player == null)// 如果当前玩家引用为空，则通过PlayerDetected方法检测并赋值
            player = PlayerDetected().transform;

        return player;// 返回玩家的Transform引用
    }

    // 检测玩家是否在射线检测范围内
    public RaycastHit2D PlayerDetected()
    {
        // 发射一条射线，检测从 playerCheck 位置开始，朝向右侧（facingDir）并且根据玩家的移动方向扩展
        // 射线长度为 playerCheckDistance，检测目标是玩家（whatIsPlayer）或地面（whatIsGround）
        RaycastHit2D hit = 
            Physics2D.Raycast(playerCheck.position, Vector2.right * facingDir, playerCheckDistance, whatIsPlayer | whatIsGround);

        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player")) // 如果没有击中任何物体，或者击中的物体不是玩家层
            return default;// 返回默认值，表示没有检测到玩家
        
        return hit;// 返回射线碰撞的结果（即检测到玩家）
    }

    // 绘制调试时显示射线的 Gizmos（在场景视图中可见）
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        
        Gizmos.color = Color.yellow;// 设置 Gizmos 的颜色为黄色
        
        // 绘制从 playerCheck 位置开始，朝向玩家检测方向的射线，长度为 playerCheckDistance
        Gizmos.DrawLine(playerCheck.position,new Vector3(playerCheck.position.x + (facingDir * playerCheckDistance),playerCheck.position.y));
        
        Gizmos.color = Color.blue;// 设置 Gizmos 的颜色为黄色
        Gizmos.DrawLine(playerCheck.position,new Vector3(playerCheck.position.x + (facingDir * attackDistance),playerCheck.position.y));
        
        Gizmos.color = Color.green;// 设置 Gizmos 的颜色为黄色
        Gizmos.DrawLine(playerCheck.position,new Vector3(playerCheck.position.x + (facingDir * minRetreatDistance),playerCheck.position.y));
    }

    private void OnEnable()
    {
        Player.OnPlayerDeath += HandlePlayerDeath;// 订阅玩家死亡事件，当玩家死亡时调用HandlePlayerDeath方法
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= HandlePlayerDeath;// 取消订阅玩家死亡事件，避免内存泄漏
    }
}
