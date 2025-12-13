using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour ,IPointerEnterHandler, IPointerExitHandler,IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;
    private UI_SkillTree skillTree;
    
    [Header("Unlock details")]
    public UI_TreeNode[] neededNodes;
    public UI_TreeNode[] conflictNodes;
    public bool isUnlocked;
    public bool isLocked;
    
    [Header("Skill details")]
    public Skill_DataSO skillData;
    [SerializeField] private string skillName;
    [SerializeField] private Image skillIcon;
    [SerializeField] private int skillCost;
    [SerializeField] private string lockedColorHex = "#737373";
    private Color lastColor;

    // 初始化时更新技能图标的颜色
    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>();
        
        UpdateIconColor(GetColorByHex(lockedColorHex));// 获取并设置图标颜色，使用十六进制颜色值
    }

    // 解锁技能
    private void Unlock()
    {
        isUnlocked = true;// 将技能状态设置为已解锁
        UpdateIconColor(Color.white);// 更新图标颜色为白色，表示解锁
        skillTree.RemoveSkillPoints(skillData.cost);
        LockConflictNodes();// 锁定冲突节点
    }

    // 判断技能是否可以解锁
    private bool CanBeUnlocked()
    {
        if (isLocked || isUnlocked) // 如果技能已锁定或已解锁，则不能解锁
            return false;
        
        if(skillTree.EnoughSkillPoints(skillData.cost) == false)// 如果技能点不足，不能解锁
            return false;

        foreach (var node in neededNodes)// 检查所有必需节点是否已解锁
        {
            if(node.isUnlocked == false)// 如果有必需节点未解锁，不能解锁
                return false;
        }

        foreach (var node in conflictNodes)// 检查是否有冲突节点已解锁
        {
            if (node.isUnlocked)// 如果有冲突节点已解锁，不能解锁
                return false;
        }
            
        return true; // 否则可以解锁
    }

    // 锁定冲突节点
    private void LockConflictNodes()
    {
        foreach (var node in conflictNodes)
            node.isLocked = true;// 将所有冲突节点锁定
    }

    // 更新技能图标的颜色
    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null)// 如果图标不存在，则不执行任何操作
            return;
        
        lastColor = skillIcon.color;// 保存上一次的图标颜色
        skillIcon.color = color;// 设置技能图标的新颜色
    }
    
    // 鼠标按下时触发
    public void OnPointerDown(PointerEventData eventData)
    {
        if(CanBeUnlocked()) // 如果技能可以解锁，调用 Unlock 解锁技能
            Unlock();
        else
            Debug.Log("Cannot be unlocked!"); // 如果不能解锁，输出提示信息
    }
    
    // 鼠标进入图标时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(true,rect,this);
        
        if(isUnlocked == false)// 如果技能没有解锁，更新图标颜色为略暗的白色
            UpdateIconColor(Color.white * 0.9f); // 调暗颜色，表示不可点击状态
    }

    // 鼠标离开图标时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false,rect);
        
        if(isUnlocked == false)// 如果技能没有解锁，恢复图标颜色为原来的颜色
            UpdateIconColor(lastColor);
    }

    // 根据十六进制字符串返回颜色
    private Color GetColorByHex(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);  // 使用 ColorUtility 来解析十六进制颜色字符串
        
        return color; // 返回解析后的颜色
    }
    
    // 在编辑器中验证时调用，用于更新技能数据
    public void OnValidate()
    {
        if (skillData == null)// 如果技能数据为空，直接返回
            return;

        skillName = skillData.displayName;// 更新技能名称
        skillIcon.sprite = skillData.icon; // 
        skillCost = skillData.cost;
        gameObject.name = "UI_TreeNode - " + skillData.displayName;// 更新对象名称
    }
}
