using System;
using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    public float damage = 10;
    
    [Header("Target detection")]
    [SerializeField] private Transform targetCheck;//目标检查
    [SerializeField] private float targetCheckRadius = 1;//目标检查半径
    [SerializeField] private LayerMask whatIsTarget;//什么是目标

    public void PerformAttack()
    {
        foreach (var target in GetDetectedColliders()) // 遍历所有检测到的碰撞体（敌人攻击范围内的目标）
        {
            IDamgable damgable = target.GetComponent<IDamgable>();
            damgable?.TakeDamage(damage,transform);
            
            // 获取目标的 Entity_Health 组件，这个组件负责管理目标的生命值
            //Entity_Health targetHealth = target.GetComponent<Entity_Health>();
            // if (targetHealth != null)
            //     targetHealth.TakeDamage(10);
            // 使用安全访问符（?.）检查 targetHealth 是否为 null，如果不为 null，则调用 TakeDamage 方法
            //targetHealth?.TakeDamage(damage,transform);// 对目标造成伤害（damage 是攻击造成的伤害值）
            
        }
    }

    // 获取攻击范围内的所有碰撞体（即目标）
    private Collider2D[] GetDetectedColliders()
    {
        // 使用 Physics2D.OverlapCircleAll 检测指定位置（targetCheck.position）和半径（targetCheckRadius）内的所有碰撞体
        // whatIsTarget 是 LayerMask，用于筛选哪些物体是有效的目标（比如敌人、玩家等）
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }
    
    // 在场景视图中绘制调试用的圆形范围，帮助可视化攻击检测范围
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position,targetCheckRadius);// 绘制一个线框圆，表示攻击检测的范围
    }
}
