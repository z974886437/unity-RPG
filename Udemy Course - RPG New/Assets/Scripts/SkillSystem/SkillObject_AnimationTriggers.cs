using System;
using UnityEngine;

public class SkillObject_AnimationTriggers : MonoBehaviour
{
    private SkillObject_TimeEcho timeEcho;

    private void Awake()
    {
        timeEcho = GetComponentInParent<SkillObject_TimeEcho>();// 在对象初始化时获取父对象上的 SkillObject_TimeEcho 组件并缓存
    }

    //攻击触发
    private void AttackTrigger()
    {
        timeEcho.PerformAttack();// 调用父对象的 PerformAttack 方法，执行一次攻击逻辑
    }

    //试试终止
    private void TryTerminate(int currentAttackIndex)
    {
        if (currentAttackIndex == timeEcho.maxAttacks)// 如果当前攻击次数达到最大攻击次数，则触发结束处理
            timeEcho.HandleDeath();// 调用父对象的 HandleDeath 方法，处理对象死亡或技能结束
    }
}
