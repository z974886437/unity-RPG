using UnityEngine;

public class Skill_DomainExpansion : Skill_Base
{

    // 判断当前领域技能是否需要“立刻生成”，用于区分不同升级分支的行为
    public bool InstantDomain()
    {
        return upgradeType != SkillUpgradeType.Domain_EchoSpan// 当升级类型不是“回声扩展”也不是“碎片扩展”时，表示可以立刻生成领域
               && upgradeType != SkillUpgradeType.Domain_ShardSpan;// 排除碎片扩展升级，因为该升级通常需要额外触发条件
    }
    
    // 创建领域技能对象，用于生成具体的领域表现与效果
    public void CreateDomain()
    {
        Debug.Log("Create skill object!");
    }
}
