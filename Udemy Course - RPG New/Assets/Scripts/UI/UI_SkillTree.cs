using UnityEngine;

public class UI_SkillTree : MonoBehaviour
{
    public int skillPoints;

    // 判断是否有足够的技能点
    public bool EnoughSkillPoints(int cost) => skillPoints >= cost;
    
    // 移除技能点
    public void RemoveSkillPoints(int cost) => skillPoints -= cost;
}
