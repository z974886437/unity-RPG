using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;//空闲状态
    public Enemy_MoveState moveState;//移动状态
    public Enemy_AttackState attackState;//攻击状态
    public Enemy_BattleState battleState;//战斗状态

    [Header("Movement details")] 
    public float idleTime = 2;//空闲时间
    public float moveSpeed = 1.4f;//移动速度
    [Range(0,2)]
    public float moveAnimSpeedMultiplier = 1;//移动动画速度倍增器
    
    [Header("Player detection")]
    [SerializeField] private LayerMask whatIsPlayer;//什么是玩家
    [SerializeField] private Transform playerCheck;//玩家检测
    [SerializeField] private float playerCheckDistance = 10;//检测玩家距离

    // 检测玩家是否在射线检测范围内
    public RaycastHit2D PlayerDetection()
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
    }
}
