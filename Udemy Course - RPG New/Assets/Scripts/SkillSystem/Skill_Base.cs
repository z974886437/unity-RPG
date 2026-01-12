using System;
using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    public Player_SkillManager skillManager { get; private set; }
    public Player player { get; private set; }
    public DamageScaleData damageScaleData { get; private set; }
    
    [Header("General Details")]
    [SerializeField] protected SkillType skillType;
    [SerializeField] protected SkillUpgradeType upgradeType;
    [SerializeField] protected float cooldown;
    private float lastTimeUsed;

    // 在对象初始化时调用
    protected virtual void Awake()
    {
        skillManager = GetComponentInParent<Player_SkillManager>();
        player = GetComponentInParent<Player>();
        lastTimeUsed = lastTimeUsed - cooldown;// 将 lastTimeUsed 设置为（当前时间 - 冷却时间），以便技能在初始时可以立即使用
        damageScaleData = new DamageScaleData();
    }

    // 尝试使用技能，子类可以重写此方法实现具体逻辑
    public virtual void TryUseSkill()
    {
        
    }

    // 设置技能升级的属性
    public void SetSkillUpgrade(UpgradeData upgrade)
    {
        upgradeType = upgrade.upgradeType;// 设置技能的升级类型
        cooldown = upgrade.cooldown;// 设置技能的冷却时间
        damageScaleData = upgrade.damageScaleData;
        ResetCooldown();
    }

    // 检查技能是否可以使用
    public virtual bool CanUseSkill()
    {
        if (upgradeType == SkillUpgradeType.None)
            return false;
        
        if (OnCooldown())// 如果技能在冷却中，则返回 false
        {
            Debug.Log("On Cooldown");
            return false;
        }
        
        return true;// 如果技能不在冷却中，则返回 true
    }

    //技能升级是否已经解锁
    protected bool Unlocked(SkillUpgradeType upgradeToCheck) => upgradeType == upgradeToCheck;

    // 判断技能是否在冷却中
    protected bool OnCooldown() => Time.time < lastTimeUsed + cooldown;
    
    // 设置技能进入冷却状态，并记录冷却开始时间
    public void SetSkillOnCooldown() => lastTimeUsed = Time.time;

    // 通过减少冷却时间来重置技能冷却（例如减少一定的冷却时间）
    public void ReduceCooldownBy(float cooldownReduction) => lastTimeUsed = lastTimeUsed + cooldownReduction;
    
    // 重置技能冷却时间，设置为当前时间
    public void ResetCooldown() => lastTimeUsed = Time.time - cooldown;
}
