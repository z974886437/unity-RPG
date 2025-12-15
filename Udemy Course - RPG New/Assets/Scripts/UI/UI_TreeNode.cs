using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour ,IPointerEnterHandler, IPointerExitHandler,IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;
    private UI_SkillTree skillTree;
    private UI_TreeConnectHandler connectHandler;
    
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
        connectHandler = GetComponent<UI_TreeConnectHandler>();
        
        UpdateIconColor(GetColorByHex(lockedColorHex));// 获取并设置图标颜色，使用十六进制颜色值
    }

    // 退还技能点
    public void Refund()
    {
        isUnlocked = false; // 将技能的解锁状态设为false
        isLocked = false;// 将技能的锁定状态设为false
        UpdateIconColor(GetColorByHex(lockedColorHex));// 更新技能图标的颜色为锁定状态的颜色
        
        skillTree.AddSkillPoints(skillData.cost);// 将花费的技能点数退还给玩家
        connectHandler.UnlockConnectionImage(false); // 更新连接图像的解锁状态为false（例如禁用图像）
    }

    // 解锁技能
    private void Unlock()
    {
        isUnlocked = true;// 将技能状态设置为已解锁
        UpdateIconColor(Color.white);// 更新图标颜色为白色，表示解锁
        skillTree.RemoveSkillPoints(skillData.cost);
        
        LockConflictNodes();// 锁定冲突节点
        connectHandler.UnlockConnectionImage(true);// 解锁连接图像并更新为解锁状态

        // 获取技能树中与当前技能类型对应的技能，并设置其升级信息
        skillTree.skillManager.GetSkillByType(skillData.skillType).SetSkillUpgrade(skillData.upgradeData);
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
        else if (isLocked)
            ui.skillToolTip.LockedSkillEffect(); // 如果不能解锁，输出提示信息
    }
    
    // 鼠标进入图标时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(true,rect,this);
        
        if(isUnlocked  || isLocked )// 如果技能没有解锁，更新图标颜色为略暗的白色
            return;
        ToggleNodeHighlight(true);
    }

    // 鼠标离开图标时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false,rect);

        if (isUnlocked || isLocked) // 如果技能没有解锁，更新图标颜色为略暗的白色
            return;
        
        ToggleNodeHighlight(false);
    }

    //切换节点高亮
    private void ToggleNodeHighlight(bool highlight)
    {
        Color highlightColor = Color.white * 0.9f; // 定义高亮颜色，稍微降低亮度（0.9倍白色）
        highlightColor.a = 1;// 设置高亮颜色的透明度为完全不透明（alpha = 1）
        Color colorToApply = highlight ? highlightColor : lastColor;// 根据 highlight 参数选择颜色，如果 highlight 为 true，则使用高亮颜色，否则使用上次的颜色
        
        UpdateIconColor(colorToApply);// 更新图标颜色
    }
    

    // 根据十六进制字符串返回颜色
    private Color GetColorByHex(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);  // 使用 ColorUtility 来解析十六进制颜色字符串
        
        return color; // 返回解析后的颜色
    }

    //关闭
    private void OnDisable()
    {
        if(isLocked)// 如果技能被锁定，则更新图标颜色为锁定状态的颜色
            UpdateIconColor(GetColorByHex(lockedColorHex));
        
        if(isUnlocked)// 如果技能已解锁，则更新图标颜色为白色（表示解锁状态）
            UpdateIconColor(Color.white);
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
