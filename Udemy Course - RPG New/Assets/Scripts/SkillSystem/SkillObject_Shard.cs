using System;
using UnityEngine;

public class SkillObject_Shard : SkillObject_Base
{

    [SerializeField] private GameObject vfxPrefab;


    // 设置碎片的爆炸延迟时间，detinationTime 为爆炸时间
    public void SetupShard(float detinationTime)
    {
        Invoke(nameof(Explode),detinationTime); // 在 detinationTime 秒后调用 Explode 方法
    }
    
    // 执行碎片爆炸
    private void Explode()
    {
        DamageEnemiesInRadius(transform,checkRadius);// 在碎片位置范围内对敌人造成伤害
        Instantiate(vfxPrefab,transform.position,Quaternion.identity);// 实例化爆炸特效（VFX）
        
        Destroy(gameObject); // 销毁碎片对象
    }
    
    // 当碎片与敌人发生碰撞时触发
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() == null)// 检查碰撞体是否是敌人，如果不是则直接返回
            return;
        
        Explode();// 如果是敌人，立即爆炸
    }
}
