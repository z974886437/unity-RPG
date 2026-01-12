using System.Collections;
using UnityEngine;

public class Skill_Shard : Skill_Base
{
    private SkillObject_Shard currentShard;
    private Entity_Health playerHealth;
    
    [SerializeField] private GameObject shardPrefab;//碎片预制
    [SerializeField] private float detonateTime = 2;//引爆时间
    
    [Header("Moving Shard Upgrade")]
    [SerializeField] private float shardSpeed = 7;
    
    [Header("Multicast Shard Upgrade")]
    [SerializeField] private int maxCharges = 3;//最大充电次数
    [SerializeField] private int currentCharges;//当前充电
    [SerializeField] private bool isRecharging;//是否充电

    [Header("Teleport Shard Upgrade")]
    [SerializeField] private float shardExistDuration = 10;//碎片存在持续时间
    
    [Header("Health Rewind Shard Upgrade")]
    [SerializeField] private float savedHealthPercent;
    

    // 覆盖基类的Awake方法，初始化技能相关属性
    protected override void Awake()
    {
        base.Awake();
        currentCharges = maxCharges;// 初始充能为最大充能
        playerHealth = GetComponentInParent<Entity_Health>();
    }
    
    // 创建一个碎片对象，并设置其爆炸时间
    public void CreateShard()
    {
        float detonationTime = GetDetonateTime();// 获取爆炸时间
        
        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);// 实例化碎片预制体，生成一个碎片对象在当前物体位置
        currentShard = shard.GetComponent<SkillObject_Shard>();// 获取碎片的组件
        currentShard.SetupShard(this);// 设置碎片的爆炸时间

        // 如果解锁了相关技能，注册碎片爆炸后的冷却回调
        if (Unlocked(SkillUpgradeType.Shard_Teleport) || Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
            currentShard.OnExplode += ForceCooldown;// 爆炸后回调 ForceCooldown，防止连续滥用传送
    }

    // 创建一个未附加升级效果的基础碎片
    public void CreateRawShard(Transform target = null ,bool shardsCanMove = false)
    {
        // 判断碎片是否具备移动能力：解锁追踪敌人或多重施法任一升级即可
        bool canMove = shardsCanMove != false ? shardsCanMove : Unlocked(SkillUpgradeType.Shard_MoveToEnemy) || Unlocked(SkillUpgradeType.Shard_Multicast);
        
        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);// 实例化碎片预制体，生成一个碎片对象在当前物体位置
        shard.GetComponent<SkillObject_Shard>().SetupShard(this,detonateTime,canMove,shardSpeed,target);// 初始化碎片参数，传入技能本体、引爆时间、是否可移动以及飞行速度
    }

    // 专用于“领域技能”生成的碎片 与普通碎片区分，避免触发玩家本体的升级联动
    public void CreateDomainShard(Transform target)
    {
        
    }

    // 尝试使用技能
    public override void TryUseSkill()
    {
        if (CanUseSkill() == false)// 如果技能不可用，则返回
            return;

        // 检查并处理不同技能升级
        if (Unlocked(SkillUpgradeType.Shard))
            HandleShardRegular();// 处理碎片升级
        
        if(Unlocked(SkillUpgradeType.Shard_MoveToEnemy))
            HandleShardMoving();// 处理碎片向敌人移动
        
        if(Unlocked(SkillUpgradeType.Shard_Multicast))
            HandleShardMulticast();// 处理碎片多重投射
        
        if(Unlocked(SkillUpgradeType.Shard_Teleport))
            HandleShardTeleport();// 处理碎片传送
        
        if(Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
            HandleShardHealthRewind();// 处理碎片传送并恢复生命
    }

    // 处理碎片恢复生命
    private void HandleShardHealthRewind()
    {
        if (currentShard == null)// 如果当前没有碎片
        {
            CreateShard();// 创建一个新的碎片
            savedHealthPercent = playerHealth.GetHealthPercent();// 保存当前生命百分比
        }
        else// 如果已有碎片
        {
            SwapPlayerAndShard();// 交换玩家与碎片的位置
            playerHealth.SetHealthToPercent(savedHealthPercent);// 恢复生命到保存的百分比
            SetSkillOnCooldown();// 设置技能进入冷却
        }
    }

    // 处理碎片传送
    private void HandleShardTeleport()
    {
        if (currentShard == null)// 如果当前没有碎片
        {
            CreateShard();// 创建一个新的碎片
        }
        else// 如果已有碎片
        {
            SwapPlayerAndShard();// 交换玩家与碎片的位置
            SetSkillOnCooldown();// 设置技能进入冷却
        }
    }

    // 交换玩家和碎片的位置
    private void SwapPlayerAndShard()
    {
        Vector3 shardPosition = currentShard.transform.position;// 获取碎片当前位置
        Vector3 playerPosition = player.transform.position; // 获取玩家当前位置
        
        currentShard.transform.position = playerPosition;// 将碎片位置设置为玩家位置
        currentShard.Explode();// 使碎片爆炸
        
        player.TeleportPlayer(shardPosition);// 玩家传送到碎片原来的位置
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
    private void HandleShardRegular()
    {
        CreateShard();// 创建碎片
        SetSkillOnCooldown();// 设置技能进入冷却状态
    }

    

    // 获取碎片的爆炸时间
    public float GetDetonateTime()
    {
        // 如果解锁了瞬移或恢复生命的技能，返回碎片存在时间作为爆炸时间
        if (Unlocked(SkillUpgradeType.Shard_Teleport) || Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
            return shardExistDuration;
        
        return detonateTime;// 否则返回默认爆炸时间
    }

    // 强制进入技能冷却状态
    private void ForceCooldown()
    {
        if (OnCooldown() == false)// 如果技能没有在冷却中
        {
            SetSkillOnCooldown(); // 进入技能冷却
            currentShard.OnExplode -= ForceCooldown;// 注销爆炸时的回调
        }
    }
}
