using UnityEngine;

public class Chest : MonoBehaviour,IDamgable
{
    private Rigidbody2D rb => GetComponentInChildren<Rigidbody2D>();
    private Animator anim => GetComponentInChildren<Animator>();
    private Entity_VFX fx => GetComponent<Entity_VFX>();
    
    [Header("Open Details")]
    [SerializeField] private Vector2 knockback;
    
    // 处理受伤逻辑
    public bool TakeDamage(float damage, Transform damageDealer)
    {
        fx.PlayOnDamageVfx(); // 播放受伤特效
        anim.SetBool("chestOpen",true);// 播放胸部打开的动画（例如受伤时角色的动作）
        rb.linearVelocity = knockback;// 设置刚体的线性速度为击退速度，实现被击退效果
        
        rb.angularVelocity = Random.Range(-200f,200f);// 设置刚体的角速度为随机值，实现随机旋转效果
        
        return true;
    }
}
