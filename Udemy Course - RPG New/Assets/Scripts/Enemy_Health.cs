using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy => GetComponent<Enemy>();
    
   
    
    public override void TakeDamage(float damage,Transform damageDealer)
    {
        base.TakeDamage(damage,damageDealer);

        if (isDead)// 如果实体已经死亡，直接返回，停止进一步处理
            return;
        
        if (damageDealer.GetComponent<Player>() != null)// 如果伤害来源是玩家，则让敌人进入战斗状态
            enemy.TryEnterBattleState(damageDealer);
        
    }
}
