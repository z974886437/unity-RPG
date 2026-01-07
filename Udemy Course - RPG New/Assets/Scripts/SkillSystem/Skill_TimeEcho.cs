using UnityEngine;

public class Skill_TimeEcho : Skill_Base
{
    [SerializeField] private GameObject timeEchoPrefab;
    [SerializeField] private float timeEchoDuration;


    // 获取时间回响技能的持续时间，供回响对象决定生命周期
    public float GetEchoDuration()
    {
        return timeEchoDuration;// 返回配置好的回响持续时间数值
    }

    // 尝试释放时间回响技能，是技能的统一入口
    public override void TryUseSkill()
    {
        if (CanUseSkill() == false) // 如果技能当前不可使用（冷却中、条件不足），直接返回
            return;
        
        CreateTimeEcho();// 条件满足后，创建一个时间回响对象
    }

    // 在当前位置生成时间回响对象，并完成其初始化
    public void CreateTimeEcho()
    {
        GameObject timeEcho = Instantiate(timeEchoPrefab, transform.position, Quaternion.identity);// 在角色当前位置实例化时间回响预制体
        timeEcho.GetComponent<SkillObject_TimeEcho>().SetupEcho(this);// 获取回响对象组件，并传入技能管理器以控制其行为和生命周期
    }
}
