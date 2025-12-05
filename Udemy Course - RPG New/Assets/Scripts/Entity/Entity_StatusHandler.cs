using System;
using System.Collections;
using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private Entity_VFX entityVFX;
    private Entity_Stats entityStats;
    private Entity_Health entityHealth;
    private ElementType currentEffect = ElementType.None;

    // 在组件启用时调用，获取相关的组件引用
    private void Awake()
    {
        entityStats = GetComponent<Entity_Stats>();
        entityHealth = GetComponent<Entity_Health>();
        entity = GetComponent<Entity>();
        entityVFX = GetComponent<Entity_VFX>();
    }

    // 应用燃烧效果，计算总伤害并启动燃烧协程
    public void ApplyBurnEffect(float duration, float fireDamage)
    {
        float fireResistance = entityStats.GetElementalResistance(ElementType.Fire);// 获取目标的火焰抗性
        float finalDamage = fireDamage * (1 - fireResistance); // 根据目标的火焰抗性计算最终伤害
        
        StartCoroutine(BurnEffectCo(duration, finalDamage)); // 启动燃烧效果协程，传入持续时间和最终伤害
    }

    // 燃烧效果协程，按时间间隔逐步减少目标生命值
    private IEnumerator BurnEffectCo(float duration, float totalDamage)
    {
        currentEffect = ElementType.Fire; // 设置当前的状态效果为火焰
        entityVFX.PlayOnStatusVfx(duration, ElementType.Fire); // 播放火焰状态的视觉效果

        int ticksPerSecond = 2; // 每秒钟造成两次伤害（ticksPerSecond 为 2）
        int tickCount = Mathf.RoundToInt(ticksPerSecond * duration); // 计算总共需要的伤害次数
        
        float damagePerTick = totalDamage / tickCount; // 每次伤害的伤害值
        float tickInterval = 1f / ticksPerSecond; // 每次伤害之间的时间间隔

        for (int i = 0; i < tickCount; i++)// 按照计算的伤害次数进行逐次伤害
        {
            entityHealth.ReduceHp(damagePerTick); // 每次伤害减少目标的生命值
            yield return new WaitForSeconds(tickInterval); // 等待设定的时间间隔后再继续
        }
        
        currentEffect = ElementType.None; // 在燃烧效果结束后，重置当前的状态效果

    }

    // 应用冰冻效果，持续时间和减速倍数作为参数
    public void ApplyChilledEffect(float duration, float slowMultiplier)
    {
        float iceResistance = entityStats.GetElementalResistance(ElementType.Ice); // 获取角色的冰霜抗性
        float finalDuration = duration * (1 - iceResistance); // 根据冰霜抗性减少效果持续时间
        
        StartCoroutine(ChilledEffectCo(finalDuration,slowMultiplier)); // 启动冰冻效果的协程
    }

    // 冰冻效果的协程，实现角色减速并播放特效
    private IEnumerator ChilledEffectCo(float duration,float slowMultiplier)
    {
        entity.SlowDownEntity(duration, slowMultiplier); // 应用角色减速效果
        currentEffect = ElementType.Ice; // 设置当前效果为冰冻
        entityVFX.PlayOnStatusVfx(duration,ElementType.Ice);  // 播放冰冻特效
        
        yield return new WaitForSeconds(duration); // 等待效果持续时间
        currentEffect = ElementType.None; // 结束效果，恢复为无状态
    }

    // 判断角色是否可以应用新的效果
    public bool CanBeApplied(ElementType element)
    {
        return currentEffect == ElementType.None; // 只有当前没有效果时，才能应用新效果
    }
}
