using System;
using UnityEngine;

public class Player_SkillManager : MonoBehaviour
{
    public Skill_Dash dash { get; private set; }
    public Skill_Shard shard { get; private set; }
    public Skill_SwordThrow swordThrow { get;private set; }
    public Skill_TimeEcho timeEcho { get;private set; }
    public Skill_DomainExpansion domainExpansion { get; private set; }//域扩展

    private Skill_Base[] allSkills;

    // 初始化并缓存玩家身上的所有技能组件，供统一管理使用
    private void Awake()
    {
        dash = GetComponentInChildren<Skill_Dash>();// 获取角色子物体上的冲刺技能组件，用于主动技能调用
        shard = GetComponentInChildren<Skill_Shard>(); // 获取碎片技能组件，用于范围伤害或引爆逻辑
        swordThrow = GetComponentInChildren<Skill_SwordThrow>();// 获取飞剑投掷技能组件，用于远程穿透攻击
        timeEcho = GetComponentInChildren<Skill_TimeEcho>();// 获取时间回响技能组件，用于复制或延迟攻击效果
        domainExpansion = GetComponentInChildren<Skill_DomainExpansion>();
        
        allSkills = GetComponentsInChildren<Skill_Base>();// 获取角色身上所有继承自 Skill_Base 的技能，用于批量操作
    }

    // 为角色的所有技能统一减少冷却时间
    public void ReduceAllSkillCooldownBy(float amount)
    {
        foreach(var skill in allSkills)// 遍历当前角色持有的所有技能实例
            skill.ReduceCooldownBy(amount);  // 对每个技能分别减少冷却时间，实现全局冷却加速效果
    }

    public Skill_Base GetSkillByType(SkillType type)
    {
        switch (type)// 根据传入的技能类型返回对应的技能对象
        {
            case SkillType.Dash: return dash; // 如果技能类型是 Dash，则返回 Dash 技能对象
            case SkillType.TimeShard: return shard;
            case SkillType.SwordThrow: return swordThrow;
            case SkillType.TimeEcho: return timeEcho;
            case SkillType.DomainExpansion: return domainExpansion;
            
            default:
                Debug.Log($"skill type {type} is not implemented yet"); // 如果技能类型未实现，输出警告日志并返回 null
                return null;
        }
    }
}
