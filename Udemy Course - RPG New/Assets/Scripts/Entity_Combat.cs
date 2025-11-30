using System;
using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    [Header("Target detection")]
    [SerializeField] private Transform targetCheck;//目标检查
    [SerializeField] private float targetCheckRadius = 1;//目标检查半径
    [SerializeField] private LayerMask whatIsTarget;//什么是目标

    public void PerformAttack()
    {
        foreach (var target in GetDetectedColliders()) // 遍历所有检测到的碰撞体（敌人攻击范围内的目标）
        {
            Debug.Log("Attacking" + target.name);
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
