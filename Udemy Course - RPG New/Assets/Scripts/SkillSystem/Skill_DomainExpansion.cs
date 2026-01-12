using System.Collections.Generic;
using UnityEngine;

public class Skill_DomainExpansion : Skill_Base
{
    [SerializeField] private GameObject domainPrefab;
    
    [Header("Slowing Down Upgrade")]
    [SerializeField] private float slowDownPercent = 0.8f;//减慢百分比
    [SerializeField] private float slowDownDomainDuration = 5;//减慢域持续时间
    
    [Header("Shard cast Upgrade")]
    [SerializeField] private int shardsToCast = 10;
    [SerializeField] private float shardCastDomainSlow = 1;//施法领域减速
    [SerializeField] private float shardCastDomainDuration = 8;//施法域持续时间
    private float spellCastTimer;
    private float spellsPerSecond;
    
    [Header("Time echo cast Upgrade")]
    [SerializeField] private int echoToCast = 8;
    [SerializeField] private float echoCastDomainSlow = 1;
    [SerializeField] private float echoCastDomainDuration = 6;
    [SerializeField] private float healthToRestoreWithEcho = 0.05f;
    
    [Header("Domain details")]
    public float maxDomainSize = 10;//最大域大小
    public float expandSpeed = 3;//扩张速度

    private List<Enemy> trappedTargets = new List<Enemy>();//被困目标
    private Transform currentTarget;

    // 创建领域技能对象，用于生成具体的领域表现与效果
    public void CreateDomain()
    {
        spellsPerSecond = GetSpellToCast() / GetDomainDuration();// 根据领域持续时间计算每秒施法次数，保证在持续时间内均匀释放技能
        
        // 在当前物体位置实例化领域预制体，不进行旋转
        GameObject domain = Instantiate(domainPrefab, // 领域技能的预制体
            transform.position,// 生成位置：当前物体的位置
            Quaternion.identity);// 不进行旋转，保持默认朝向
        
        domain.GetComponent<SkillObject_DomainExpansion>().SetupDomain(this);// 获取领域脚本并传入当前技能管理器，用于初始化领域参数
    }
    
    // 负责在领域存在期间持续进行施法逻辑
    public void DoSpellCasting()
    {
        spellCastTimer -= Time.deltaTime;// 使用帧时间递减施法计时器，保证与帧率无关

        if (currentTarget == null) // 如果当前没有目标，则尝试从领域内寻找一个
            currentTarget = FindTargetInDomain();

        if (currentTarget != null && spellCastTimer < 0)// 当存在目标且计时器结束时才进行施法
        {
            CastSpell(currentTarget);// 对当前目标释放对应的领域衍生技能
            spellCastTimer = 1 / spellsPerSecond;// 重置施法计时器，1 / 每秒施法次数 = 两次施法的间隔时间
            currentTarget = null;// 清空当前目标，确保下次重新随机
        }
    }

    // 根据领域升级类型，对目标释放不同的衍生技能
    private void CastSpell(Transform target)
    {
        if (upgradeType == SkillUpgradeType.Domain_EchoSpan)// 若升级为“时间回响扩散”，则在目标附近生成时间回响
        {
            Vector3 offset = Random.value < 0.5f ? new Vector3(1, 0) : new Vector3(-1, 0);// 随机选择目标左侧或右侧偏移，避免完全重叠生成
            
            skillManager.timeEcho.CreateTimeEcho(target.position + offset); // 在目标附近创建时间回响实例
        }

        if (upgradeType == SkillUpgradeType.Domain_ShardSpan)// 若升级为“碎片扩散”，则直接对目标生成原始碎片
        {
            skillManager.shard.CreateRawShard(target,true); // true 表示该碎片来源于领域，而非普通技能释放
        }
    }

    // 从被领域困住的目标列表中随机选取一个
    private Transform FindTargetInDomain()
    {
        // 移除所有无效目标（已被销毁或死亡）
        trappedTargets.RemoveAll(target => target == null || target.health.isDead);
        
        if(trappedTargets.Count == 0)// 如果列表为空，则返回 null，表示当前领域没有可攻击目标
            return null;

        int randomIndex = Random.Range(0, trappedTargets.Count); // 从有效目标列表中随机选择一个索引
        return trappedTargets[randomIndex].transform;// 返回选中目标的 Transform，用于后续施法逻辑
    }
    
    // 根据当前升级类型，获取领域的持续时间
    public float GetDomainDuration()
    {
        if (upgradeType == SkillUpgradeType.Domain_SlowingDown)// 若当前升级为“减速领域”，则使用对应的持续时间
            return slowDownDomainDuration;
        else if(upgradeType == SkillUpgradeType.Domain_ShardSpan)// 若当前升级为“碎片扩散领域”，则使用碎片施法领域持续时间
            return shardCastDomainDuration;
        else if (upgradeType == SkillUpgradeType.Domain_EchoSpan)// 若当前升级为“回响扩散领域”，则使用回响施法领域持续时间
            return echoCastDomainDuration;

        return 0;// 若未匹配任何领域升级类型，则返回 0，表示无效领域
    }

    // 根据当前升级类型，获取领域的减速百分比
    public float GetSlowPercentage()
    {
        if (upgradeType == SkillUpgradeType.Domain_SlowingDown)// 若为减速领域，则返回其专属减速百分比
            return slowDownPercent;
        else if(upgradeType == SkillUpgradeType.Domain_ShardSpan) // 若为碎片施法领域，则返回该领域附带的减速效果
            return shardCastDomainSlow;
        else if (upgradeType == SkillUpgradeType.Domain_EchoSpan)// 若为回响施法领域，则返回该领域附带的减速效果
            return echoCastDomainSlow;

        return 0; // 未匹配到领域升级类型时，不附加减速
    }

    // 根据当前领域升级类型，获取领域期间施法的总次数
    private int GetSpellToCast()
    {
        if(upgradeType == SkillUpgradeType.Domain_ShardSpan)// 若为碎片扩散领域，则返回碎片施法次数
            return shardsToCast;
        else if (upgradeType == SkillUpgradeType.Domain_EchoSpan)// 若为回响扩散领域，则返回回响施法次数
            return echoToCast;

        return 0;// 非施法类领域不进行施法
    }

    // 判断当前领域技能是否需要“立刻生成”，用于区分不同升级分支的行为
    public bool InstantDomain()
    {
        return upgradeType != SkillUpgradeType.Domain_EchoSpan// 当升级类型不是“回声扩展”也不是“碎片扩展”时，表示可以立刻生成领域
               && upgradeType != SkillUpgradeType.Domain_ShardSpan;// 排除碎片扩展升级，因为该升级通常需要额外触发条件
    }

    // 将进入领域的敌人加入“被困目标列表”
    public void AddTarget(Enemy targetToAdd)
    {
        trappedTargets.Add(targetToAdd);// 把敌人加入列表，用于后续领域内的施法与减速控制
    }

    // 清空领域内的所有目标，并恢复它们的状态
    public void ClearTargets()
    {
        foreach(var enemy in trappedTargets)// 遍历当前所有被领域影响的敌人
            enemy.StopSlowDown();// 停止敌人身上的减速效果，避免领域消失后仍被影响

        trappedTargets = new List<Enemy>();// 重新创建列表而不是 Clear() 原因：防止其他地方持有旧列表引用导致逻辑错误
    }
}
