using System;
using UnityEngine;

public class Entity_AnimationTriggers : MonoBehaviour
{
    private Entity entity;
    private Entity_Combat entityCombat;

    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityCombat = GetComponentInParent<Entity_Combat>();
    }

    // 当前状态触发的方法，调用实体的动画触发器
    private void CurrentStateTrigger()
    {
        entity.CurrentStateAnimationTrigger();// 触发当前状态的动画更新
    }

    // 攻击触发的方法，调用实体的攻击执行方法
    private void AttackTrigger()
    {
        entityCombat.PerformAttack();// 执行攻击操作
    }
}
