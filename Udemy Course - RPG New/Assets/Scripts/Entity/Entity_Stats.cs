using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat maxHealth;
    public Stat_MajorGroup major;
    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defense;


    // 获取物理伤害并判断是否暴击
    public float GetPhyiscalDamage(out bool isCrit)
    {
        // 基础伤害 = 攻击力的基础伤害 + 主要属性的力量加成
        float baseDamage = offense.damage.GetValue(); // 获取攻击力基础伤害
        float bonusDamage = major.strength.GetValue();// 获取主要属性力量的加成
        float totalBaseDamage = baseDamage + bonusDamage;// 计算总基础伤害

        // 暴击几率 = 攻击力的暴击几率 + 主要属性敏捷加成（敏捷的0.3倍）
        float baseCritChance = offense.critChance.GetValue();// 获取基础暴击几率
        float bonusCritChance = major.agility.GetValue() * 0.3f; // 获取敏捷带来的暴击几率加成
        float critChance = baseCritChance + bonusCritChance; // 计算总暴击几率

        // 暴击伤害 = 攻击力的暴击伤害 + 主要属性力量加成（力量的0.5倍）
        float baseCritPower = offense.critPower.GetValue();// 获取基础暴击伤害
        float bonusCritPower = major.strength.GetValue() * 0.5f; // 获取力量带来的暴击伤害加成
        float critPower = (baseCritPower + bonusCritPower) / 100;// 计算暴击伤害倍率

        isCrit = Random.Range(0, 100) < critChance;// 如果随机值小于暴击几率，则判定为暴击
        float finalDamage = isCrit ? totalBaseDamage * critPower : totalBaseDamage; // 计算最终伤害，若暴击则伤害乘以暴击倍率

        return finalDamage;// 返回最终伤害值
    }

    // 计算护甲缓解效果的方法，接受一个参数：护甲减免（从敌方攻击者获得的护甲穿透）
    public float GetArmorMitigation(float armorReduction)
    {
        float baseArmor = defense.armor.GetValue();// 获取基础护甲值（通常是角色本身的防御属性）
        float bonusArmor = major.vitality.GetValue();  // 获取额外护甲值（通常是角色的生命值或其他属性提供的加成）
        float totalArmor = baseArmor + bonusArmor;// 计算总护甲（基础护甲 + 加成护甲）

        float reductionMutliplier = Mathf.Clamp(1 - armorReduction,0,1);// 计算护甲减免倍数，确保它的范围在 0 到 1 之间（防止减免大于 100%）
        float effectiveArmor = totalArmor * reductionMutliplier; // 根据护甲减免倍数，计算实际护甲值

        float mitigation = effectiveArmor / (effectiveArmor + 100);// 根据有效护甲计算伤害减免值，公式是有效护甲 / (有效护甲 + 100)
        float mitigationCap = 0.85f;// 设置最大缓解值为 85%（即护甲无法完全消除伤害，最多能减少 85%）

        float finalMitigation = Mathf.Clamp(mitigation, 0, mitigationCap);// 确保伤害减免值不会超过最大缓解值

        return finalMitigation;// 返回最终的护甲减免效果
    }

    // 计算敌方的护甲减免效果，返回一个值用于减轻护甲的效果
    public float GetArmorReduction()
    {
        float finalReduction = offense.armorReduction.GetValue() / 100;// 获取攻击者的护甲减免百分比（通常是敌人的某些攻击属性）
        
        return finalReduction; // 返回最终的护甲减免效果（以比例的形式，0.2 代表 20% 的穿透）
    }

    // 获取闪避值
    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue(); // 获取基础闪避值
        float bonusEvasion = major.agility.GetValue() * 0.5f;//each agility point gives you 0.5% of evasion;根据敏捷值计算额外的闪避值，每点敏捷增加0.5%的闪避

        float totalEvasion = baseEvasion + bonusEvasion;// 计算总闪避值
        float evasionCap = 85f;// 设置闪避值的上限为85%

        float finalEvasion = Mathf.Clamp(totalEvasion, 0, evasionCap); // 返回闪避值，确保不超过上限（0到85之间）
        
        return finalEvasion;
    }
    
    // 获取最大生命值
    public float GetMaxHealth()
    {
        float baseMaxHealth = maxHealth.GetValue();// 获取基础生命值
        float bonusMaxHealth = major.vitality.GetValue() * 5;// 每点体质增加5点生命值
        float finalMaxHealth = baseMaxHealth + bonusMaxHealth;
        
        return finalMaxHealth;// 返回基础生命值加上额外生命值
    }
}
