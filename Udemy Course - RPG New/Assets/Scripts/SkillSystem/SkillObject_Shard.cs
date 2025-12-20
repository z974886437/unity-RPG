using System;
using UnityEngine;

public class SkillObject_Shard : SkillObject_Base
{
    public event Action OnExplode;
    private Skill_Shard shardManager;
    
    [SerializeField] private GameObject vfxPrefab;

    private Transform target;
    private float speed;


    // 每帧更新，移动物体
    private void Update()
    {
        if (target == null)// 如果目标为空，直接返回
            return;

        // 将物体朝目标移动
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    // 移动物体朝最近的目标
    public void MoveTowardsClosestTarget(float speed)
    {
        target = FindClosestTarget();// 获取最近的目标
        this.speed = speed; // 设置物体的移动速度
    }

    // 设置碎片的爆炸延迟时间，detinationTime 为爆炸时间
    public void SetupShard(Skill_Shard shardManager)
    {
        this.shardManager = shardManager;// 缓存碎片所属的技能管理器，便于访问技能数据

        playerStats = shardManager.player.stats;// 从技能管理器中获取玩家属性，用于后续伤害计算
        damageScaleData = shardManager.damageScaleData;// 从技能管理器中获取伤害倍率数据，用于元素与数值缩放

        float detonationTime = shardManager.GetDetonateTime();// 从技能管理器中获取碎片的引爆时间，便于统一由技能控制
        
        Invoke(nameof(Explode),detonationTime); // 在指定引爆时间后自动调用 Explode，实现延迟爆炸效果
    }

    // 初始化碎片（增强版本：支持移动与自定义引爆时间）
    public void SetupShard(Skill_Shard shardManager, float detonationTime, bool canMove, float shardSpeed)
    {
        this.shardManager = shardManager;// 缓存碎片所属的技能管理器，确保碎片与技能数据绑定
        playerStats = shardManager.player.stats;// 从技能管理器中获取玩家属性，用于碎片造成的伤害计算
        damageScaleData = shardManager.damageScaleData;// 从技能管理器中获取伤害倍率数据，用于元素与数值缩放
        
        Invoke(nameof(Explode),detonationTime); // 使用外部传入的引爆时间调用 Explode，支持升级或特殊逻辑控制
        
        if(canMove)// 如果碎片允许移动，则朝最近的敌人移动
            MoveTowardsClosestTarget(shardSpeed);
    }
    
    // 执行碎片爆炸
    public void Explode()
    {
        DamageEnemiesInRadius(transform,checkRadius);// 在碎片位置范围内对敌人造成伤害
        GameObject vfx = Instantiate(vfxPrefab,transform.position,Quaternion.identity);// 实例化爆炸特效（VFX）
        vfx.GetComponentInChildren<SpriteRenderer>().color = shardManager.player.vfx.GetElementColor(usedElement);
        
        OnExplode?.Invoke();// 触发爆炸事件（如果有订阅）
        Destroy(gameObject); // 销毁碎片对象
    }
    
    // 当碎片与敌人发生碰撞时触发
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() == null)// 检查碰撞体是否是敌人，如果不是则直接返回
            return;
        
        Explode();// 如果是敌人，立即爆炸
    }
}
