using System.Collections;
using UnityEngine;

public class Skill_Shard : Skill_Base
{
    private SkillObject_Shard currentShard;
    
    [SerializeField] private GameObject shardPrefab;//碎片预制
    [SerializeField] private float detonateTime = 2;//引爆时间
    
    [Header("Moving Shard Upgrade")]
    [SerializeField] private float shardSpeed = 7;
    
    [Header("Multicast Shard Upgrade")]
    [SerializeField] private int maxCharges = 3;//最大充电次数
    [SerializeField] private int currentCharges;//当前充电
    [SerializeField] private bool isRecharging;//是否充电


    // 覆盖基类的Awake方法，初始化技能相关属性
    protected override void Awake()
    {
        base.Awake();
        currentCharges = maxCharges;// 初始充能为最大充能
    }

    // 尝试使用技能
    public override void TryUseSkill()
    {
        if (CanUseSkill() == false)// 如果技能不可用，则返回
            return;

        // 检查并处理不同技能升级
        if (Unlocked(SkillUpgradeType.Shard))
            HandleShardUpgrade();// 处理碎片升级
        
        if(Unlocked(SkillUpgradeType.Shard_MoveToEnemy))
            HandleShardMoving();// 处理碎片向敌人移动
        
        if(Unlocked(SkillUpgradeType.Shard_Multicast))
            HandleShardMulticast();// 处理碎片多重投射
    }

    // 处理多重投射碎片技能
    private void HandleShardMulticast()
    {
        if (currentCharges < 0)// 如果充能不足，则返回
            return;
        
        CreateShard();// 创建一个新的碎片
        currentShard.MoveTowardsClosestTarget(shardSpeed);// 碎片向最近敌人移动
        currentCharges--;// 减少充能

        if (isRecharging == false)
            StartCoroutine(ShardRechargeCo());// 启动充能协程
    }

    // 碎片技能充能协程
    private IEnumerator ShardRechargeCo()
    {
        isRecharging = true;// 设置为正在充能

        while (currentCharges < maxCharges)// 循环充能直到达到最大充能
        {
            yield return new WaitForSeconds(cooldown); // 等待冷却时间
            currentCharges++;// 增加充能
        }

        isRecharging = false; // 充能结束
    }

    // 处理碎片向敌人移动的技能
    private void HandleShardMoving()
    {
        CreateShard();// 创建碎片
        currentShard.MoveTowardsClosestTarget(shardSpeed);// 碎片向目标移动
        
        SetSkillOnCooldown();// 设置技能进入冷却状态
    }

    // 处理碎片升级的技能
    private void HandleShardUpgrade()
    {
        CreateShard();// 创建碎片
        SetSkillOnCooldown();// 设置技能进入冷却状态
    }

    // 创建一个碎片对象，并设置其爆炸时间
    public void CreateShard()
    {
        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);// 实例化碎片预制体，生成一个碎片对象在当前物体位置
        currentShard = shard.GetComponent<SkillObject_Shard>();// 获取碎片的组件
        currentShard.SetupShard(detonateTime);// 设置碎片的爆炸时间
    }
}
