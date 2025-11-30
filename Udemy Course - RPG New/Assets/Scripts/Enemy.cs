using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;//空闲状态
    public Enemy_MoveState moveState;//移动状态
    public Enemy_AttackState attackState;//攻击状态

    [Header("Movement details")] 
    public float idleTime = 2;//空闲时间
    public float moveSpeed = 1.4f;//移动速度
    [Range(0,2)]
    public float moveAnimSpeedMultiplier = 1;//移动动画速度倍增器
}
