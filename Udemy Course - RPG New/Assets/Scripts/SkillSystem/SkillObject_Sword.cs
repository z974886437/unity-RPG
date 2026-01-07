using System;
using UnityEngine;

public class SkillObject_Sword : SkillObject_Base
{
    protected Skill_SwordThrow swordManager;
    

    protected Transform playerTransform;
    protected bool shouldComeback;//应该回归
    protected float comebackSpeed = 20;
    protected float maxAllowedDistance = 25;//最大允许距离

    // 每帧调用，用于更新剑的朝向并处理回收逻辑
    protected virtual void Update()
    {
        transform.right = rb.linearVelocity; // 根据刚体当前速度方向旋转剑，使剑头始终朝向飞行方向
        HandleComeback();// 每帧检测并执行飞剑回到玩家的逻辑
    }

    // 初始化飞剑的核心数据（由技能管理器调用）
    public virtual void SetupSword(Skill_SwordThrow swordManager,Vector2 direction)
    {
        
        rb.linearVelocity = direction;// 设置飞剑初始飞行速度（方向已在外部归一化或计算）
        
        this.swordManager = swordManager;// 保存技能管理器引用，用于后续交互或回调

        playerTransform = swordManager.transform.root;// 获取玩家根节点 Transform，用于回收飞剑时定位
        playerStats = swordManager.player.stats;// 缓存玩家属性数据，用于伤害或状态计算
        damageScaleData = swordManager.damageScaleData;// 缓存伤害缩放数据，用于技能成长或数值平衡
    }
    
    // 对外接口：通知飞剑开始返回玩家
    public void GetSwordBackToPlayer() => shouldComeback = true;

    // 处理飞剑回到玩家的移动与销毁逻辑
    protected void HandleComeback()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);// 计算飞剑与玩家之间的距离，用于判断是否回收完成
        
        if (shouldComeback == false)// 如果尚未触发回收状态，则直接退出
            return;

        // 以固定速度将飞剑移动向玩家位置（不依赖物理系统）
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, comebackSpeed * Time.deltaTime);

        if (distance < 0.5f) // 当飞剑足够接近玩家时，销毁对象完成回收
            Destroy(gameObject);
    }

    // 飞剑触发碰撞时调用（如命中敌人或场景物体）
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        StopSword(collision); // 停止飞剑并将其固定在命中的目标上
        DamageEnemiesInRadius(transform,1);  // 在命中点范围内对敌人造成伤害
    }

    // 停止飞剑的飞行并附着在碰撞物体上
    protected void StopSword(Collider2D collision)
    {
        rb.simulated = false;// 关闭刚体模拟，防止飞剑继续受到物理影响
        transform.parent = collision.transform; // 将飞剑设置为碰撞物体的子物体，实现“插在目标上”的效果
    }
    
}
