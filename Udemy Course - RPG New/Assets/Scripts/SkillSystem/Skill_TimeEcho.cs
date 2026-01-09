using UnityEngine;

public class Skill_TimeEcho : Skill_Base
{
    [SerializeField] private GameObject timeEchoPrefab;
    [SerializeField] private float timeEchoDuration;
    
    [Header("Attack Upgrades")]
    [SerializeField] private int maxAttacks = 3;
    [SerializeField] private float duplicateChance = 0.3f;//重复机会

    [Header("Heal wisp Upgrades")]
    [SerializeField] private float damagePercentHealed = 0.3f;//伤害治疗百分比
    [SerializeField] private float cooldownReducedInSeconds;//冷却时间在几秒内缩短


    // 获取回响转化为幽魂后，可按伤害比例转化的治疗百分比
    public float GetPercentOfDamageHealed()
    {
        if (ShouldBeWisp() == false) // 如果当前升级类型不支持幽魂形态，则不提供任何治疗收益
            return 0;
        
        return damagePercentHealed;// 返回幽魂基于伤害值进行治疗的百分比系数
    }

    // 获取幽魂回收时可减少的技能冷却时间（秒）
    public float GetCooldownReduceInSeconds()
    {
        if (upgradeType != SkillUpgradeType.TimeEcho_CooldownWisp) // 只有冷却型幽魂升级时，才允许提供冷却缩减效果
            return 0;
        
        return cooldownReducedInSeconds;// 返回幽魂回收后实际减少的冷却时间数值
    }

    // 判断幽魂回收时是否具备清除负面状态的能力
    public bool CanRemoveNegativeEffects()
    {
        return upgradeType == SkillUpgradeType.TimeEcho_CleanseWisp; // 仅当升级为净化型幽魂时，才允许清除负面效果
    }

    // 判断当前回响在死亡后是否应转化为幽魂形态
    public bool ShouldBeWisp()
    {
        // 只要升级类型属于任意幽魂分支，就进入幽魂逻辑
        return upgradeType == SkillUpgradeType.TimeEcho_HealWisp
               || upgradeType == SkillUpgradeType.TimeEcho_CleanseWisp
               || upgradeType == SkillUpgradeType.TimeEcho_CooldownWisp;
    }
    
    //获得重复机会
    public float GetDuplicateChance()
    {
        if (upgradeType != SkillUpgradeType.TimeEcho_ChanceToDuplicate)// 如果当前升级类型不是时间回响复制几率，则返回0
            return 0;
        
        return duplicateChance; // 返回当前技能的复制几率
    }
    
    //获得最大攻击
    public int GetMaxAttacks()
    {
        // 如果升级类型是单次攻击或复制几率类型，则最大攻击次数为1
        if(upgradeType == SkillUpgradeType.TimeEcho_SingleAttack || upgradeType == SkillUpgradeType.TimeEcho_ChanceToDuplicate)
            return 1;
        
        // 如果升级类型是多次攻击，则返回配置的最大攻击次数
        if(upgradeType == SkillUpgradeType.TimeEcho_MultiAttack)
            return maxAttacks;

        return 0;// 默认情况返回0，表示无法攻击
    }

    // 获取时间回响技能的持续时间，供回响对象决定生命周期
    public float GetEchoDuration()
    {
        return timeEchoDuration;// 返回配置好的回响持续时间数值
    }

    // 尝试释放时间回响技能，是技能的统一入口
    public override void TryUseSkill()
    {
        if (CanUseSkill() == false) // 如果技能当前不可使用（冷却中、条件不足），直接返回
            return;
        
        CreateTimeEcho();// 条件满足后，创建一个时间回响对象
    }

    // 在当前位置生成时间回响对象，并完成其初始化
    public void CreateTimeEcho(Vector3? targetPosition = null)
    {
        Vector3 position = targetPosition ?? transform.position; // 如果传入了目标位置就用它，否则使用自己当前位置
        
        GameObject timeEcho = Instantiate(timeEchoPrefab, position, Quaternion.identity);// 在角色当前位置实例化时间回响预制体
        timeEcho.GetComponent<SkillObject_TimeEcho>().SetupEcho(this);// 获取回响对象组件，并传入技能管理器以控制其行为和生命周期
    }
}
