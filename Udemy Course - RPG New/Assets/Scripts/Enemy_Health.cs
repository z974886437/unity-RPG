using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy => GetComponent<Enemy>();
    
    public override void TakeDamage(float damage,Transform damageDealer)
    {
        if (damageDealer.GetComponent<Player>() != null)// 如果伤害来源是玩家，则让敌人进入战斗状态
            enemy.TryEnterBattleState(damageDealer);
        
        base.TakeDamage(damage,damageDealer);
    }
}
