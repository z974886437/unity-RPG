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

    // 获取最大生命值
    public float GetMaxHealth()
    {
        float baseMaxHealth = maxHealth.GetValue();// 获取基础生命值
        float bonusMaxHealth = major.vitality.GetValue() * 5;// 每点体质增加5点生命值
        float finalMaxHealth = baseMaxHealth + bonusMaxHealth;
        
        return finalMaxHealth;// 返回基础生命值加上额外生命值
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
}
