using System.Text;
using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{
    private UI_SkillTree skillTree;
    
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;//技能描述
    [SerializeField] private TextMeshProUGUI skillRequirements;//技能要求

    [Space]
    [SerializeField] private string metConditionHex;//条件为十六进制
    [SerializeField] private string notMetConditionHex;
    [SerializeField] private string importantInfoHex;
    [SerializeField] private Color exampleColor;//示例颜色
    [SerializeField] private string lockedSkillText = "你选择了不同分支 - 此分支已锁定";

    protected override void Awake()
    {
        base.Awake();

        skillTree = GetComponentInParent<UI_SkillTree>();
    }

    public override void ShowToolTip(bool show, RectTransform targetRect)
    {
        base.ShowToolTip(show, targetRect);
    }

    // 显示技能的详细信息
    public void ShowToolTip(bool show, RectTransform targetRect,UI_TreeNode node)
    {
        base.ShowToolTip(show, targetRect);

        if (show == false)// 如果不显示工具提示，直接返回
            return;

        skillName.text = node.skillData.displayName;// 设置技能名称
        skillDescription.text = node.skillData.description;// 设置技能描述

        string skillLockedText = $"<color={importantInfoHex}>{lockedSkillText} </color>";
        string requirements = node.isLocked ? skillLockedText : GetRequirements(node.skillData.cost,node.neededNodes,node.conflictNodes);
        
        skillRequirements.text = requirements;// 设置技能要求

    }

    // 获取技能要求的字符串，包括技能点、必需节点和冲突节点
    private string GetRequirements(int skillCost,UI_TreeNode[] neededNodes,UI_TreeNode[] conflictNodes)
    {
        StringBuilder sb = new StringBuilder();// 创建 StringBuilder 用于构建字符串

        sb.AppendLine("要求:");// 添加要求的标题
        
        string costColor = skillTree.EnoughSkillPoints(skillCost) ? metConditionHex : notMetConditionHex;// 判断技能点是否足够，设置颜色

        sb.AppendLine($"<color={costColor}> - {skillCost} 技能点 </color>");// 添加技能点要求并设置颜色

        foreach (var node in neededNodes)    // 添加所有必需节点信息，设置已解锁或未解锁颜色
        {
            string nodeColor = node.isUnlocked ? metConditionHex : notMetConditionHex;
            sb.AppendLine($"<color={nodeColor}> - {node.skillData.displayName} </color>");
        }

        if(conflictNodes.Length <= 0)// 如果没有冲突节点，直接返回
            return sb.ToString();

        sb.AppendLine();// 添加空行
        sb.AppendLine($"<color={importantInfoHex}> 无法选择： </color>");// 添加冲突节点标题

        foreach (var node in conflictNodes) // 添加所有冲突节点信息
        {
            sb.AppendLine($"<color={importantInfoHex}> - {node.skillData.displayName} </color>");
        }
        
        return sb.ToString(); // 返回构建的字符串
    }
}
