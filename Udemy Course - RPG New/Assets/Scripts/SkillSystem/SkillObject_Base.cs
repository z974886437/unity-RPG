using UnityEngine;

public class SkillObject_Base : MonoBehaviour
{
    [SerializeField] protected LayerMask whatIsEnemy;
    [SerializeField] protected Transform targetCheck;
    [SerializeField] protected float checkRadius = 1;

    protected Entity_Stats playerStats;
    protected DamageScaleData damageScaleData;
    protected ElementType usedElement;


    // 在指定半径内对所有敌人造成伤害
    protected void DamageEnemiesInRadius(Transform t, float radius)
    {
        foreach (var target in EnemiesAround(t, radius))// 遍历半径范围内的所有敌人
        {
            IDamgable damgable = target.GetComponent<IDamgable>();// 获取敌人是否可受伤（实现IDamgable接口）

            if (damgable == null)// 如果敌人不可受伤，跳过当前循环
                continue;

            ElementalEffectData effectData = new ElementalEffectData(playerStats, damageScaleData); // 创建一个元素效果数据，包含玩家的属性和伤害加成数据

            float physDamage = playerStats.GetPhyiscalDamage(out bool isCrit, damageScaleData.phyiscal);// 计算物理伤害，并判断是否为暴击
            float elemDamage = playerStats.GetElementalDamage(out ElementType element, damageScaleData.elemental);// 计算元素伤害，并获取元素类型

            damgable.TakeDamage(physDamage, elemDamage, element, transform);// 对敌人造成物理伤害和元素伤害，伤害类型为元素类型，伤害来源为当前物体
            
            if(element != ElementType.None)// 如果有元素伤害，应用相应的状态效果
                target.GetComponent<Entity_StatusHandler>().ApplyStatusEffect(element,effectData);

            usedElement = element;
        }
    }

    // 查找离当前物体最近的敌人
    protected Transform FindClosestTarget()
    {
        Transform target = null;// 初始化目标为null
        float closesDistance = Mathf.Infinity; // 设置初始最小距离为无穷大

        // 遍历周围的敌人
        foreach (var enemy in EnemiesAround(transform, 10))
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
    protected Collider2D[] EnemiesAround(Transform t, float radius)
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
