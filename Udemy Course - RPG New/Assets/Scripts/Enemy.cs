using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;//空闲状态
    public Enemy_MoveState moveState;//移动状态
    public Enemy_AttackState attackState;//攻击状态
    public Enemy_BattleState battleState;//战斗状态
    public Enemy_DeadState deadState;//死亡状态
    
    [Header("Battle details")]
    public float battleMoveSpeed = 3;//战斗移动速度
    public float attackDistance = 2;//攻击距离
    public float battleTimeDuration = 5;//战斗持续时间
    public float minRetreatDistance = 1;//最小后退距离
    public Vector2 retrealVelocity;//后退速度
    
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


    public override void EntityDeath()
    {
        base.EntityDeath();
        
        stateMachine.ChangeState(deadState);
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
}
