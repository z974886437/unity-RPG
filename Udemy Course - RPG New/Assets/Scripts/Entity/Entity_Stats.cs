using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat maxHealth;
    public Stat_MajorGroup major;
    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defense;
    

    // 获取最大生命值
    public float GetMaxHealth()
    {
        float baseHp = maxHealth.GetValue();// 获取基础生命值
        float bonusHp = major.vitality.GetValue() * 5;// 每点体质增加5点生命值
        
        return baseHp + bonusHp;// 返回基础生命值加上额外生命值
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
