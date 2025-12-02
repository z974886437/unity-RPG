
using UnityEngine;
using UnityEngine.UI;

public class Entity_Health : MonoBehaviour,IDamgable
{
    private Slider healthBar;
    private Entity_VFX entityVfx;
    private Entity entity;
    
    [SerializeField] protected float currentHp;
    [SerializeField] protected float maxHp = 100;
    [SerializeField] protected bool isDead;
    
    [Header("On Damage Knockback")]
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f,2.5f);//关于伤害击退
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(7, 7);//重伤害击退
    [SerializeField] private float knockbackDuration = 0.2f;//击退持续时间
    [SerializeField] private float heavyKnockbackDuration = 0.5f;//重击退持续时间
    [Header("On Heavy Damage")]
    [SerializeField] private float heavyDamageThreshold = 0.3f;//重损伤阈值

    protected void Awake()
    {
        entityVfx = GetComponent<Entity_VFX>();
        entity = GetComponent<Entity>();
        healthBar = GetComponentInChildren<Slider>();
        
        currentHp = maxHp;
        UpdateHealthBar();
    }

    // 处理实体受到伤害的方法
    public virtual void TakeDamage(float damage,Transform damageDealer)
    {
        if (isDead) // 如果实体已经死亡，直接返回，不再处理伤害
            return;
        
        Vector2 knockback = CalculateKnockback(damage,damageDealer);// 计算击退方向和距离
        float duration = CalculateDuration(damage);// 计算根据伤害决定的击退持续时间
        
        entity?.ReciveKnockback(knockback,duration);// 如果存在实体对象，执行击退动作
        entityVfx?.PlayOnDamageVfx();// 如果存在伤害视觉效果对象，播放伤害效果
        ReduceHp(damage);// 调用 ReduceHp 方法扣除生命值
    }

    // 扣除生命值的方法
    protected void ReduceHp(float damage)
    {
        currentHp -= damage;// 减少当前生命值（maxHp 可能是当前生命值的变量名称）
        UpdateHealthBar();

        if (currentHp <= 0)// 如果生命值小于 0，调用 Die 方法执行死亡逻辑
            Die();
    }

    // 死亡处理方法
    private void Die()
    {
        isDead = true;// 标记实体为已死亡
        entity?.EntityDeath();
    }

    // 更新血条的显示
    private void UpdateHealthBar()
    {
        if (healthBar == null)// 如果血条对象为空，直接返回
            return;
        
        healthBar.value = currentHp / maxHp; // 设置血条的值，值为当前生命值与最大生命值的比值
    }
        

    //计算击退
    private Vector2 CalculateKnockback(float damage,Transform damageDealer)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;// 根据攻击者与实体的位置，确定击退方向，左边攻击为负，右边攻击为正

        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackPower : knockbackPower; // 判断是否为重击，根据伤害选择不同的击退效果
        knockback.x = knockback.x * direction;// 调整击退方向，确保与攻击者相反
        
        return knockback;// 返回计算好的击退向量
    }

    //计算持续时间
    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;// 判断是否为重击，选择不同的击退持续时间
    
    private bool IsHeavyDamage(float damage) => damage / maxHp > heavyDamageThreshold;// 判断伤害是否超过重击阈值
}
