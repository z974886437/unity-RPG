using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{
    private UI ui;
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

    private Coroutine textEffectCo;

    protected override void Awake()
    {
        base.Awake();

        ui = GetComponentInParent<UI>();
        skillTree = ui.GetComponentInChildren<UI_SkillTree>(true);
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

        string skillLockedText = GetColoredText(importantInfoHex, lockedSkillText);
        string requirements = node.isLocked ? skillLockedText : GetRequirements(node.skillData.cost,node.neededNodes,node.conflictNodes);
        
        skillRequirements.text = requirements;// 设置技能要求

    }

    //锁定技能效果
    public void LockedSkillEffect()
    {
        if(textEffectCo != null)// 如果文本效果协程正在运行，则停止当前协程
            StopCoroutine(textEffectCo);

        textEffectCo = StartCoroutine(TextBlinkEffectCO(skillRequirements, 0.15f, 3));// 启动新的文本闪烁效果协程（每隔0.15秒闪烁3次）
    }
    
    //文本闪烁效果 CO
    private IEnumerator TextBlinkEffectCO(TextMeshProUGUI text,float blinkInterval,int blinkCount)
    {
        for (int i = 0; i < blinkCount; i++)// 循环执行指定次数的闪烁效果
        {
            text.text = GetColoredText(notMetConditionHex, lockedSkillText);// 设置文本为锁定状态的颜色（文本颜色为 notMetConditionHex）
            yield return new WaitForSeconds(blinkInterval); // 等待指定的闪烁间隔时间（如0.15秒）

            text.text = GetColoredText(importantInfoHex, lockedSkillText);// 设置文本为重要信息颜色（文本颜色为 importantInfoHex）
            yield return new WaitForSeconds(blinkInterval);// 等待指定的闪烁间隔时间
        }
    }

    // 获取技能要求的字符串，包括技能点、必需节点和冲突节点
    private string GetRequirements(int skillCost,UI_TreeNode[] neededNodes,UI_TreeNode[] conflictNodes)
    {
        StringBuilder sb = new StringBuilder();// 创建 StringBuilder 用于构建字符串

        sb.AppendLine("要求:");// 添加要求的标题
        
        string costColor = skillTree.EnoughSkillPoints(skillCost) ? metConditionHex : notMetConditionHex;// 判断技能点是否足够，设置颜色
        string costText = $"- {skillCost} 技能点";
        string filnalCostText = GetColoredText(costColor, costText);

        sb.AppendLine(filnalCostText);// 添加技能点要求并设置颜色

        foreach (var node in neededNodes)    // 添加所有必需节点信息，设置已解锁或未解锁颜色
        {
            string nodeColor = node.isUnlocked ? metConditionHex : notMetConditionHex;
            string nodeText = $"- {node.skillData.displayName}";
            string filnalNodeText = GetColoredText(nodeColor, nodeText);
            sb.AppendLine(filnalNodeText);
        }

        if(conflictNodes.Length <= 0)// 如果没有冲突节点，直接返回
            return sb.ToString();

        sb.AppendLine();// 添加空行
        //sb.AppendLine($"<color={importantInfoHex}> 无法选择： </color>");// 添加冲突节点标题
        sb.AppendLine(GetColoredText(importantInfoHex, "无法选择："));// 添加冲突节点标题

        foreach (var node in conflictNodes) // 添加所有冲突节点信息
        {
            string nodeText = $"- {node.skillData.displayName}";
            string finalNodeText = GetColoredText(importantInfoHex, nodeText);
            sb.AppendLine(finalNodeText);
        }
        
        return sb.ToString(); // 返回构建的字符串
    }
}
