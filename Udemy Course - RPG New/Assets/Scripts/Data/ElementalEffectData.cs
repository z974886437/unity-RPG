using System;
using UnityEngine;

public class ElementalEffectData
{
    public float chillDuration;//冷时长
    public float chillSlowMultiplier;//冷静慢速效应

    public float burnDuration;//燃烧持续时间
    public float totalBurnDamage;

    public float shockDuration;
    public float shockDamage;
    public float shockCharge;//闪电冲击

    // 构造函数，用于初始化元素效果数据（如寒冷、燃烧、闪电效果的持续时间和伤害）
    public ElementalEffectData(Entity_Stats entityStats,DamageScaleData damageScale)
    {
        chillDuration = damageScale.chillDuration;// 初始化寒冷效果的持续时间
        chillSlowMultiplier = damageScale.chillSlowMultiplier;// 初始化寒冷效果的减速倍数

        burnDuration = damageScale.burnDuration;// 初始化燃烧效果的持续时间
        totalBurnDamage = entityStats.offense.fireDamage.GetValue() * damageScale.burnDamageScale;// 计算总燃烧伤害：通过玩家的火焰伤害和燃烧伤害加成计算
        
        shockDuration = damageScale.shockDuration;// 初始化闪电效果的持续时间
        shockDamage = entityStats.offense.lightningDamage.GetValue() * damageScale.shockDamageScale; // 计算闪电伤害：通过玩家的闪电伤害和闪电伤害加成计算
        shockCharge = damageScale.shockCharge;// 初始化闪电效果的充能值
    }
}

