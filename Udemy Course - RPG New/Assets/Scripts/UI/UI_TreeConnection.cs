using UnityEngine;

public class UI_TreeConnection : MonoBehaviour
{
    [SerializeField] private RectTransform rotationPoint;
    [SerializeField] private RectTransform connectionLength;
    [SerializeField] private RectTransform childNodeConnectionPoint;

    // 根据连接方向和长度设置连接
    public void DirectConnection(NodeDirectionType direction, float length)
    {
        bool shouldBeActive = direction != NodeDirectionType.None;// 判断连接是否有效，若方向不为“None”则为有效
        float finalLength = shouldBeActive ? length : 0f;// 如果连接有效，则使用指定的长度，否则设为0
        float angle = GetDirectionAngle(direction);// 获取连接方向的角度

        rotationPoint.localRotation = Quaternion.Euler(0, 0, angle); // 设置连接的旋转角度
        connectionLength.sizeDelta = new Vector2(finalLength, connectionLength.sizeDelta.y); // 设置连接的长度
    }

    // 获取连接点的本地坐标
    public Vector2 GetConnectionPoint(RectTransform rect)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle
            (
                rect.parent as RectTransform,// RectTransform父级，用于参考坐标变换
                childNodeConnectionPoint.position,// 屏幕坐标点（通常为世界空间位置）
                null,// 相机参数，如果为null，则使用当前主相机
                out var localPosition// 输出转换后的本地坐标
            );

        return localPosition;// 返回转换后的本地坐标
    }

    // 根据连接方向返回相应的旋转角度
    private float GetDirectionAngle(NodeDirectionType type)
    {
        switch (type)
        {
            case NodeDirectionType.UpLeft : return 135f;
            case NodeDirectionType.Up : return 90f;
            case NodeDirectionType.UpRight : return 45f;
            case NodeDirectionType.Left : return 180f;
            case NodeDirectionType.Right : return 0f;
            case NodeDirectionType.DownLeft : return -135f;
            case NodeDirectionType.Down : return -90f;
            case NodeDirectionType.DownRight : return -45f;
            default : return 0f;
        }
    }
}

// 连接方向的枚举类型
public enum NodeDirectionType
{
    None,
    UpLeft,
    Up,
    UpRight,
    Left,
    Right,
    DownLeft,
    Down,
    DownRight
}