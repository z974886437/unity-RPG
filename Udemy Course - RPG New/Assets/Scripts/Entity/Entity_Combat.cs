using System;
using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    private Entity_VFX vfx;
    private Entity_Stats stats;
    
    
    [Header("Target detection")]
    [SerializeField] private Transform targetCheck;//目标检查
    [SerializeField] private float targetCheckRadius = 1;//目标检查半径
    [SerializeField] private LayerMask whatIsTarget;//什么是目标
    
    [Header("Status effect details")]
    [SerializeField] private float defaultDuration = 3;
    [SerializeField] private float chillSlowMutiplier = 0.2f;

    private void Awake()
    {
        vfx = GetComponent<Entity_VFX>();
        stats = GetComponent<Entity_Stats>();
    }

    // 执行攻击操作
    public void PerformAttack()
    {
        foreach (var target in GetDetectedColliders()) // 遍历所有检测到的碰撞体（敌人攻击范围内的目标）
        {
            IDamgable damageable = target.GetComponent<IDamgable>(); // 获取目标的 IDamgable 接口（如果目标可以被攻击）

            if (damageable == null) // 如果目标没有实现 IDamgable 接口，跳过此目标
                continue;

            float elementalDamage = stats.GetElementalDamage(out ElementType element);// 获取元素伤害和元素类型
            float damage = stats.GetPhyiscalDamage(out bool isCrit);// 获取物理伤害，并判断是否暴击
            bool targetGotHit = damageable.TakeDamage(damage,elementalDamage,element,transform);// 尝试对目标造成伤害，并检查是否成功（目标是否受到伤害）
            
            if(element != ElementType.None) // 如果攻击是元素攻击（非无效元素），应用状态效果
                ApplyStatusEffect(target.transform,element);

            if (targetGotHit) // 如果目标成功受到伤害，创建攻击命中的特效（VFX）
            {
                vfx.UpdateOnHitColor(element);// 更新特效的颜色（可能根据元素类型改变）
                vfx.CreateOnHitVFX(target.transform,isCrit); // 创建攻击命中的特效
            }
            
            // 获取目标的 Entity_Health 组件，这个组件负责管理目标的生命值
            //Entity_Health targetHealth = target.GetComponent<Entity_Health>();
            // if (targetHealth != null)
            //     targetHealth.TakeDamage(10);
            // 使用安全访问符（?.）检查 targetHealth 是否为 null，如果不为 null，则调用 TakeDamage 方法
            //targetHealth?.TakeDamage(damage,transform);// 对目标造成伤害（damage 是攻击造成的伤害值）
            
        }
    }

    // 应用状态效果到目标
    public void ApplyStatusEffect(Transform target,ElementType element)
    {
        Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();// 获取目标的状态管理器（负责处理目标的状态效果）

        if (statusHandler == null)// 如果目标没有状态管理器，直接返回
            return;
        
        if(element == ElementType.Ice && statusHandler.CanBeApplied(ElementType.Ice))// 如果元素类型是冰霜，且目标能够接受冰霜效果，则应用冰冻效果
            statusHandler.ApplyChilledEffect(defaultDuration,chillSlowMutiplier); // 应用冰冻效果，使用默认的持续时间和减速倍率
            
    }

    // 获取攻击范围内的所有碰撞体（即目标）
    protected Collider2D[] GetDetectedColliders()
    {
        // 使用 Physics2D.OverlapCircleAll 检测指定位置（targetCheck.position）和半径（targetCheckRadius）内的所有碰撞体
        // whatIsTarget 是 LayerMask，用于筛选哪些物体是有效的目标（比如敌人、玩家等）
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }
    
    // 在场景视图中绘制调试用的圆形范围，帮助可视化攻击检测范围
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position,targetCheckRadius);// 绘制一个线框圆，表示攻击检测的范围
    }
}
