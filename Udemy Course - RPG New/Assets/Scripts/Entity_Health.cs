using UnityEngine;

public class Entity_Health : MonoBehaviour
{
    [SerializeField] protected float maxHp = 100;
    [SerializeField] protected bool isDead;

    // 处理实体受到伤害的方法
    public virtual void TakeDamage(float damage,Transform damageDealer)
    {
        if (isDead) // 如果实体已经死亡，直接返回，不再处理伤害
            return;
        
        ReduceHp(damage);// 调用 ReduceHp 方法扣除生命值
    }

    // 扣除生命值的方法
    protected void ReduceHp(float damage)
    {
        maxHp -= damage;// 减少当前生命值（maxHp 可能是当前生命值的变量名称）


        if (maxHp < 0)// 如果生命值小于 0，调用 Die 方法执行死亡逻辑
            Die();
    }

    // 死亡处理方法
    private void Die()
    {
        isDead = true;// 标记实体为已死亡
        Debug.Log("实体死亡"); // 打印死亡信息
    }
}
