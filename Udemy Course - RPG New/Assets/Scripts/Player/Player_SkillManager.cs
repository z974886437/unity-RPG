using System;
using UnityEngine;

public class Player_SkillManager : MonoBehaviour
{
    public Skill_Dash dash { get; private set; }
    public Skill_Shard shard { get; private set; }
    public Skill_SwordThrow swordThrow { get;private set; }

    private void Awake()
    {
        dash = GetComponentInChildren<Skill_Dash>();
        shard = GetComponentInChildren<Skill_Shard>();
        swordThrow = GetComponentInChildren<Skill_SwordThrow>();
    }

    public Skill_Base GetSkillByType(SkillType type)
    {
        switch (type)// 根据传入的技能类型返回对应的技能对象
        {
            case SkillType.Dash: return dash; // 如果技能类型是 Dash，则返回 Dash 技能对象
            case SkillType.TimeShard: return shard;
            
            default:
                Debug.Log($"skill type {type} is not implemented yet"); // 如果技能类型未实现，输出警告日志并返回 null
                return null;
        }
    }
}
