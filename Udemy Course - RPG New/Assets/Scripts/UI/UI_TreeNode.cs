using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour ,IPointerEnterHandler, IPointerExitHandler,IPointerDownHandler
{

    [SerializeField] private Image skillIcon;
    [SerializeField] private string lockedColorHex = "#737373";
    private Color lastColor;
    public bool isUnlocked;
    public bool isLocked;

    // 初始化时更新技能图标的颜色
    private void Awake()
    {
        UpdateIconColor(GetColorByHex(lockedColorHex));// 获取并设置图标颜色，使用十六进制颜色值
    }

    // 解锁技能
    private void Unlock()
    {
        isUnlocked = true;// 将技能状态设置为已解锁
        UpdateIconColor(Color.white);// 更新图标颜色为白色，表示解锁
    }

    // 判断技能是否可以解锁
    private bool CanBeUnlocked()
    {
        if (isLocked || isUnlocked) // 如果技能已锁定或已解锁，则不能解锁
            return false;
        
        return true; // 否则可以解锁
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
        if(isUnlocked == false)// 如果技能没有解锁，更新图标颜色为略暗的白色
            UpdateIconColor(Color.white * 0.9f); // 调暗颜色，表示不可点击状态
    }

    // 鼠标离开图标时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        if(isUnlocked == false)// 如果技能没有解锁，恢复图标颜色为原来的颜色
            UpdateIconColor(lastColor);
    }

    // 根据十六进制字符串返回颜色
    private Color GetColorByHex(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);  // 使用 ColorUtility 来解析十六进制颜色字符串
        
        return color; // 返回解析后的颜色
    }
    
}
