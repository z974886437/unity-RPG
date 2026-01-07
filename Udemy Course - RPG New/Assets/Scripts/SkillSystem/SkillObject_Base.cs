using System;
using UnityEngine;

public class SkillObject_Base : MonoBehaviour
{
    [SerializeField] private GameObject onHitVfx;
    [Space]
    [SerializeField] protected LayerMask whatIsEnemy;
    [SerializeField] protected Transform targetCheck;
    [SerializeField] protected float checkRadius = 1;

    
    protected Rigidbody2D rb;
    protected Animator anim;
    protected Entity_Stats playerStats;
    protected DamageScaleData damageScaleData;
    protected ElementType usedElement;
    protected bool targetGotHit;


    protected void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();// 获取飞剑自身的 Rigidbody2D，用于物理移动
    }

    // 在指定半径内对所有敌人造成伤害
    protected void DamageEnemiesInRadius(Transform t, float radius)
    {
        foreach (var target in GetEnemiesAround(t, radius))// 遍历半径范围内的所有敌人
        {
            IDamgable damgable = target.GetComponent<IDamgable>();// 获取敌人是否可受伤（实现IDamgable接口）

            if (damgable == null)// 如果敌人不可受伤，跳过当前循环
                continue;

            AttackData attackData = playerStats.GetAttackData(damageScaleData);// 从玩家属性获取攻击数据（包括物理伤害、元素伤害、暴击等）
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();// 获取敌人的状态管理组件，用于施加元素状态效果

            float physDamage = attackData.phyiscalDamage;// 计算物理伤害，并判断是否为暴击
            float elemDamage = attackData.elementalDamage;// 计算元素伤害，并获取元素类型
            ElementType element = attackData.element;

            targetGotHit = damgable.TakeDamage(physDamage, elemDamage, element, transform);// 对敌人造成物理伤害和元素伤害，伤害类型为元素类型，伤害来源为当前物体
            
            if(element != ElementType.None)// 如果有元素伤害，应用相应的状态效果
                statusHandler?.ApplyStatusEffect(element,attackData.effectData);

            if (targetGotHit)// 如果敌人被成功击中，播放命中特效
                Instantiate(onHitVfx, target.transform.position, Quaternion.identity);

            usedElement = element;// 记录当前使用的元素类型，可用于技能或特效逻辑
        }
    }

    // 查找离当前物体最近的敌人
    protected Transform FindClosestTarget()
    {
        Transform target = null;// 初始化目标为null
        float closesDistance = Mathf.Infinity; // 设置初始最小距离为无穷大

        // 遍历周围的敌人
        foreach (var enemy in GetEnemiesAround(transform, 10))
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);// 计算当前物体与敌人之间的距离

            // 如果当前敌人的距离小于最小距离，更新目标
            if (distance < closesDistance)
            {
                target = enemy.transform;// 更新目标为当前敌人
                closesDistance = distance;// 更新最小距离
            }
        }

        return target; // 返回找到的最近敌人
    }

    // 获取指定transform位置半径范围内的所有敌人
    protected Collider2D[] GetEnemiesAround(Transform t, float radius)
    {
        return Physics2D.OverlapCircleAll(t.position, radius, whatIsEnemy);// 使用OverlapCircleAll方法检测所有敌人
    }

    // 可视化Gizmos，帮助调试显示范围
    protected virtual void OnDrawGizmos()
    {
        if (targetCheck == null)// 如果没有指定目标检查位置，则默认使用当前transform位置
            targetCheck = transform;
        
        Gizmos.DrawWireSphere(targetCheck.position,checkRadius); // 绘制一个圆形的Gizmo表示检查范围
    }
}
