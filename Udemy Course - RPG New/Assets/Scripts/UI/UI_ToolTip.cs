using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    private RectTransform rect;
    [SerializeField] private Vector2 offset = new Vector2(300,20);

    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    // 显示或隐藏工具提示
    public virtual void ShowToolTip(bool show, RectTransform targetRect)
    {
        if (show == false)// 不显示时，将工具提示移出屏幕
        {
            rect.position = new Vector2(9999, 9999); // 将工具提示的位置设置为屏幕外
            return; // 结束方法
        }
        
        UpdatePosition(targetRect); // 显示时更新工具提示的位置
    }
    
    // 更新工具提示的位置
    private void UpdatePosition(RectTransform targetRect)
    {
        float screenCenterX = Screen.width / 2f;// 获取屏幕水平中心位置
        float screenTop = Screen.height; // 获取屏幕顶部位置
        float screenBottom = 0;// 屏幕底部为 0
        
        Vector2 targetPosition = targetRect.position; // 获取目标位置
        
        // 根据目标的水平位置调整工具提示的位置，避免覆盖目标元素
        targetPosition.x = targetPosition.x > screenCenterX ? targetPosition.x - offset.x : targetPosition.x + offset.x;

        float veritcalHalf = rect.sizeDelta.y / 2f;// 获取工具提示的垂直半高
        float topY = targetPosition.y + veritcalHalf;// 工具提示的顶部位置
        float bottomY = targetPosition.y - veritcalHalf; // 工具提示的底部位置

        if (topY > screenTop) // 如果工具提示超出屏幕顶部，调整其位置
            targetPosition.y = screenTop - veritcalHalf - offset.y;// 将工具提示移至屏幕顶部之下
        else if (bottomY < screenBottom)// 如果工具提示超出屏幕底部，调整其位置
            targetPosition.y = screenBottom + veritcalHalf + offset.y;// 将工具提示移至屏幕底部之上
        
        rect.position = targetPosition; // 更新工具提示的位置
    }
    
    protected string GetColoredText(string color, string text)
    {
        return $"<color={color}>{text} </color>";
    }
}
