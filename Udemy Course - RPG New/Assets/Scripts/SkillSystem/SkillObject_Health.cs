using UnityEngine;

public class SkillObject_Health : Entity_Health
{
    protected override void Die()
    {
        SkillObject_TimeEcho timeEcho = GetComponent<SkillObject_TimeEcho>();// 从当前对象上获取时间回响组件，用于处理回响死亡表现
        timeEcho.HandleDeath();// 主动调用回响的死亡处理，生成特效并销毁对象
    }
}
