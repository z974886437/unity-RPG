using UnityEngine;

public class Skill_DomainExpansion : Skill_Base
{
    [SerializeField] private GameObject domainPrefab;
    
    [Header("Slowing Down Upgrade")]
    [SerializeField] private float slowDownPercent = 0.8f;//减慢百分比
    [SerializeField] private float slowDownDomainDuration = 5;//减慢域持续时间
    
    [Header("Spell Casting Upgrade")]
    [SerializeField] private float spellCastingDomainSlowDown = 1;//施法领域减速
    [SerializeField] private float spellCastingDomainDuration = 8;//施法域持续时间
    
    [Header("Domain details")]
    public float maxDomainSize = 10;//最大域大小
    public float expandSpeed = 3;//扩张速度



    // 根据当前升级类型，获取领域的持续时间
    public float GetDomainDuration()
    {
        if (upgradeType == SkillUpgradeType.Domain_SlowingDown)// 如果是减速领域升级
            return slowDownDomainDuration;// 返回减速领域的持续时间
        else
            return spellCastingDomainDuration;// 否则返回施法领域的持续时间
    }

    // 根据当前升级类型，获取领域的减速百分比
    public float GetSlowPercentage()
    {
        if (upgradeType == SkillUpgradeType.Domain_SlowingDown) // 如果是减速领域升级
            return slowDownPercent; // 返回减速领域的减速比例
        else
            return spellCastingDomainSlowDown;// 否则返回施法领域的减速比例
    }

    // 判断当前领域技能是否需要“立刻生成”，用于区分不同升级分支的行为
    public bool InstantDomain()
    {
        return upgradeType != SkillUpgradeType.Domain_EchoSpan// 当升级类型不是“回声扩展”也不是“碎片扩展”时，表示可以立刻生成领域
               && upgradeType != SkillUpgradeType.Domain_ShardSpan;// 排除碎片扩展升级，因为该升级通常需要额外触发条件
    }
    
    // 创建领域技能对象，用于生成具体的领域表现与效果
    public void CreateDomain()
    {
        // 在当前物体位置实例化领域预制体，不进行旋转
        GameObject domain = Instantiate(domainPrefab, // 领域技能的预制体
            transform.position,// 生成位置：当前物体的位置
            Quaternion.identity);// 不进行旋转，保持默认朝向
        
        domain.GetComponent<SkillObject_DomainExpansion>().SetupDomain(this);// 获取领域脚本并传入当前技能管理器，用于初始化领域参数
    }
}
