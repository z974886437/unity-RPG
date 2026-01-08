using UnityEngine;

public class Skill_TimeEcho : Skill_Base
{
    [SerializeField] private GameObject timeEchoPrefab;
    [SerializeField] private float timeEchoDuration;
    
    [Header("Attack Upgrades")]
    [SerializeField] private int maxAttacks = 3;
    [SerializeField] private float duplicateChance = 0.3f;//重复机会


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
