using UnityEngine;

public class Skill_Dash : Skill_Base
{


    // 在技能开始时调用的效果处理
    public void OnStartEffect()
    {
        // 如果解锁了 Dash_CloneOnStart 或 Dash_CloneOnStartAndArrival 升级，创建克隆
        if (Unlocked(SkillUpgradeType.Dash_CloneOnStart) || Unlocked(SkillUpgradeType.Dash_CloneOnStartAndArrival))
            CreateClone();
        
        // 如果解锁了 Dash_ShardOnStart 或 Dash_ShardOnStartAndArrival 升级，创建时间碎片
        if(Unlocked(SkillUpgradeType.Dash_ShardOnShart) || Unlocked(SkillUpgradeType.Dash_ShardOnStartAndArrival))
            CreateShard();
    }

    // 在技能结束时调用的效果处理
    public void OnEndEffect()
    {
        // 如果解锁了 Dash_CloneOnStartAndArrival 升级，创建克隆
        if(Unlocked(SkillUpgradeType.Dash_CloneOnStartAndArrival))
            CreateClone();
        
        // 如果解锁了 Dash_ShardOnStartAndArrival 升级，创建时间碎片
        if(Unlocked(SkillUpgradeType.Dash_ShardOnStartAndArrival))
            CreateShard();
    }

    // 创建时间碎片
    private void CreateShard()
    {
        skillManager.shard.CreateRawShard();
    }

    // 创建时间回声
    private void CreateClone()
    {
        skillManager.timeEcho.CreateTimeEcho();
    }
}
