using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;//技能描述
    [SerializeField] private TextMeshProUGUI skillRequirements;//技能要求

    
    public override void ShowToolTip(bool show, RectTransform targetRect)
    {
        base.ShowToolTip(show, targetRect);
    }

    // 显示技能的详细信息
    public void ShowToolTip(bool show, RectTransform targetRect, Skill_DataSO skillData)
    {
        base.ShowToolTip(show, targetRect);

        if (show == false)// 如果不显示工具提示，直接返回
            return;

        skillName.text = skillData.displayName;// 设置技能名称
        skillDescription.text = skillData.description;// 设置技能描述
        skillRequirements.text = "要求: \n" + " - " + skillData.cost + " 技能点 ";// 设置技能要求

    }
}
