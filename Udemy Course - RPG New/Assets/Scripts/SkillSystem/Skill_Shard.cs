using UnityEngine;

public class Skill_Shard : Skill_Base
{
    [SerializeField] private GameObject shardPrefab;//碎片预制
    [SerializeField] private float detonateTime = 2;//引爆时间



    // 创建一个碎片对象，并设置其爆炸时间
    public void CreateShard()
    {
        if (upgradeType == SkillUpgradeType.None)
            return;
        
        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);// 实例化碎片预制体，生成一个碎片对象在当前物体位置
        shard.GetComponent<SkillObject_Shard>().SetupShard(detonateTime);// 获取碎片对象的 SkillObject_Shard 组件并设置爆炸时间
        
    }
}
