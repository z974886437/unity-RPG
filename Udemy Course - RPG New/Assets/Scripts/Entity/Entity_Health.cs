
using UnityEngine;
using UnityEngine.UI;

public class Entity_Health : MonoBehaviour,IDamgable
{
    private Slider healthBar;
    private Entity entity;
    private Entity_VFX entityVfx;
    private Entity_Stats entityStats;
    
    [SerializeField] protected float currentHealth;
    [SerializeField] protected bool isDead;
    [Header("Health regen")]
    [SerializeField] private float regenInterval = 1;
    [SerializeField] private bool canRegenerateHealth = true;
    
    [Header("On Damage Knockback")]
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f,2.5f);//关于伤害击退
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(7, 7);//重伤害击退
    [SerializeField] private float knockbackDuration = 0.2f;//击退持续时间
    [SerializeField] private float heavyKnockbackDuration = 0.5f;//重击退持续时间
    [Header("On Heavy Damage")]
    [SerializeField] private float heavyDamageThreshold = 0.3f;//重损伤阈值

    protected void Awake()
    {
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();
        healthBar = GetComponentInChildren<Slider>();
        
        currentHealth = entityStats.GetMaxHealth();
        UpdateHealthBar();
        
        InvokeRepeating(nameof(RegenerateHealth),0,regenInterval);
    }

    // 处理实体受到伤害的方法
    public virtual bool TakeDamage(float damage,float elementalDamage,ElementType element,Transform damageDealer)
    {
        if (isDead) // 如果实体已经死亡，直接返回，不再处理伤害
            return false;

        if (AttackEvaded())// 检查攻击是否被闪避 
        {
            Debug.Log($"{gameObject.name} evaded the attack!");
            return false;
        }
        
        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();// 获取攻击方的属性，计算穿甲伤害的减少量
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0; // 获取攻击者的护甲穿透效果（护甲减免百分比）

        float mitigation = entityStats.GetArmorMitigation(armorReduction);// 计算护甲的减免效果
        float physicalDamageTaken = damage * (1 - mitigation); // 根据护甲减免计算最终伤害

        float resistance = entityStats.GetElementalResistance(element);// 获取当前元素的抗性（每种元素可能有不同的抗性值）
        float elementalDamageTaken = elementalDamage * (1 - resistance);// 根据元素抗性计算实际承受的元素伤害
        
        TakeKnockback(damageDealer, physicalDamageTaken);// 计算击退效果，根据伤害处理击退（可能是根据物理伤害或其他因素）
        ReduceHealth(physicalDamageTaken + elementalDamageTaken);// 调用 ReduceHp 方法扣除生命值

        return true;// 返回成功处理伤害
    }

    // 判断攻击是否被闪避
    private bool AttackEvaded() => Random.Range(0, 100) < entityStats.GetEvasion();
    
    // 恢复生命值的方法
    private void RegenerateHealth()
    {
        if (canRegenerateHealth == false)// 如果不能恢复生命值，直接返回
            return;

        float regenAmount = entityStats.resources.healthRegen.GetValue(); // 获取生命恢复量
        IncreaseHealth(regenAmount);// 增加生命值
    }

    // 增加生命值的方法
    public void IncreaseHealth(float healAmount)
    {
        if (isDead)// 如果角色已经死亡，则不执行恢复生命值的操作
            return;

        float newHealth = currentHealth + healAmount; // 计算恢复后的新生命值
        float maxHealth = entityStats.GetMaxHealth();// 获取角色的最大生命值

        currentHealth = Mathf.Min(newHealth, maxHealth);// 设置当前生命值为新生命值和最大生命值中的较小值，避免超过最大生命值
        UpdateHealthBar();// 更新生命值显示条
    }
    
    // 扣除生命值的方法
    public void ReduceHealth(float damage)
    {
        entityVfx?.PlayOnDamageVfx();// 如果存在伤害视觉效果对象，播放伤害效果
        currentHealth = currentHealth - damage;// 减少当前生命值（maxHp 可能是当前生命值的变量名称）
        UpdateHealthBar();

        if (currentHealth <= 0)// 如果生命值小于 0，调用 Die 方法执行死亡逻辑
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
        
        healthBar.value = currentHealth / entityStats.GetMaxHealth(); // 设置血条的值，值为当前生命值与最大生命值的比值
    }
        
    private void TakeKnockback(Transform damageDealer, float finalDamage)
    {
        Vector2 knockback = CalculateKnockback(finalDamage,damageDealer);// 计算击退方向和距离
        float duration = CalculateDuration(finalDamage);// 计算根据伤害决定的击退持续时间
        
        entity?.ReciveKnockback(knockback,duration);// 如果存在实体对象，执行击退动作
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
    
    private bool IsHeavyDamage(float damage) => damage / entityStats.GetMaxHealth() > heavyDamageThreshold;// 判断伤害是否超过重击阈值
}
