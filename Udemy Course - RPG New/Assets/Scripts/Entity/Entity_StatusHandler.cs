using System;
using System.Collections;
using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private Entity_VFX entityVFX;
    private Entity_Stats stats;
    private ElementType currentEffect = ElementType.None;

    // 在组件启用时调用，获取相关的组件引用
    private void Awake()
    {
        stats = GetComponent<Entity_Stats>();
        entity = GetComponent<Entity>();
        entityVFX = GetComponent<Entity_VFX>();
    }

    // 应用冰冻效果，持续时间和减速倍数作为参数
    public void ApplyChilledEffect(float duration, float slowMultiplier)
    {
        float iceResistance = stats.GetElementalResistance(ElementType.Ice); // 获取角色的冰霜抗性
        float reducedDuration = duration * (1 - iceResistance); // 根据冰霜抗性减少效果持续时间
        
        StartCoroutine(ChilledEffectCo(reducedDuration,slowMultiplier)); // 启动冰冻效果的协程
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
