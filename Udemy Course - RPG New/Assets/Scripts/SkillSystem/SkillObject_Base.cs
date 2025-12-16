using UnityEngine;

public class SkillObject_Base : MonoBehaviour
{
    [SerializeField] protected LayerMask whatIsEnemy;
    [SerializeField] protected Transform targetCheck;
    [SerializeField] protected float checkRadius = 1;



    // 在指定半径内对所有敌人造成伤害
    protected void DamageEnemiesInRadius(Transform t, float radius)
    {
        foreach (var target in EnemiesAround(t, radius))// 遍历半径范围内的所有敌人
        {
            IDamgable damgable = target.GetComponent<IDamgable>();// 获取敌人是否可受伤（实现IDamgable接口）

            if (damgable == null)// 如果敌人不可受伤，跳过当前循环
                continue;

            damgable.TakeDamage(1, 1, ElementType.None, transform);// 对敌人造成1点伤害，伤害类型为None，伤害来源为当前transform
        }
    }

    // 获取指定transform位置半径范围内的所有敌人
    protected Collider2D[] EnemiesAround(Transform t, float radius)
    {
        return Physics2D.OverlapCircleAll(t.position, radius, whatIsEnemy);// 使用OverlapCircleAll方法检测所有敌人
    }

    // 可视化Gizmos，帮助调试显示范围
    protected virtual void OnDrawGizmos()
    {
        if (targetCheck == null)// 如果没有指定目标检查位置，则默认使用当前transform位置
            targetCheck = transform;
        
        Gizmos.DrawWireSphere(targetCheck.position,checkRadius); // 绘制一个圆形的Gizmo表示检查范围
    }
}
