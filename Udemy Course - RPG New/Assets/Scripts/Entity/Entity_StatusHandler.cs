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
    
    [Header("Electrify effect details")]
    [SerializeField] private GameObject lightningStrikeVfx;
    [SerializeField] private float currentCharge;
    [SerializeField] private float maximumCharge = 1;
    private Coroutine electrifyCo;

    // 在组件启用时调用，获取相关的组件引用
    private void Awake()
    {
        entityStats = GetComponent<Entity_Stats>();
        entityHealth = GetComponent<Entity_Health>();
        entity = GetComponent<Entity>();
        entityVFX = GetComponent<Entity_VFX>();
    }

    // 应用电击效果，计算电击积蓄并可能触发雷电攻击
    public void ApplyElectrifyEffect(float duration,float damage,float charge)
    {
        float lightningResistance = entityStats.GetElementalResistance(ElementType.Lightning);// 获取目标的雷电抗性
        float finalCharge = charge * (1 - lightningResistance);// 根据目标的雷电抗性计算电击积蓄的最终值
        currentCharge = currentCharge + finalCharge;// 增加当前电击积蓄

        if (currentCharge >= maximumCharge)// 如果电击积蓄达到最大值，则触发雷电攻击并停止电击效果
        {
            DoLightningStrike(damage);// 执行雷电攻击
            StopElectrifyEffect();// 停止电击效果
            return;
        }
        
        if(electrifyCo != null) // 如果电击效果协程正在运行，停止它
            StopCoroutine(electrifyCo);

        electrifyCo = StartCoroutine(ElectrifyEffectCo(duration)); // 启动新的电击效果协程
    }

    // 停止电击效果，重置状态
    private void StopElectrifyEffect()
    {
        currentEffect = ElementType.None;// 重置当前状态效果
        currentCharge = 0; // 重置电击积蓄
        entityVFX.StopAllVfx(); // 停止所有电击效果的视觉效果
    }

    // 执行雷电攻击，造成伤害并显示雷电特效
    private void DoLightningStrike(float damage)
    {
        Instantiate(lightningStrikeVfx, transform.position, Quaternion.identity);// 在目标位置生成雷电特效
        entityHealth.ReduceHealth(damage); // 减少目标的生命值，造成伤害
    }

    // 电击效果的协程，持续时间内应用电击状态效果
    private IEnumerator ElectrifyEffectCo(float duration)
    {
        currentEffect = ElementType.Lightning;// 设置当前效果为雷电
        entityVFX.PlayOnStatusVfx(duration,ElementType.Lightning); // 播放雷电状态的视觉效果
        
        yield return new WaitForSeconds(duration); // 等待电击效果持续时间
        StopElectrifyEffect();// 停止电击效果
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
            entityHealth.ReduceHealth(damagePerTick); // 每次伤害减少目标的生命值
            yield return new WaitForSeconds(tickInterval); // 等待设定的时间间隔后再继续
        }
        
        currentEffect = ElementType.None; // 在燃烧效果结束后，重置当前的状态效果

    }

    // 应用冰冻效果，持续时间和减速倍数作为参数
    public void ApplyChillEffect(float duration, float slowMultiplier)
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
        if (element == ElementType.Lightning && currentEffect == ElementType.Lightning)
            return true;
        
        return currentEffect == ElementType.None; // 只有当前没有效果时，才能应用新效果
    }
}
