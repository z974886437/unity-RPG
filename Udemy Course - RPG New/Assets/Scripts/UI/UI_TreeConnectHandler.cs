using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

[Serializable]
public class UI_TreeConnectDetails
{
    public UI_TreeConnectHandler childNode;
    public NodeDirectionType direction;
    [Range(100f, 350f)] public float length;
    [Range(-50f, 50f)] public float rotation;
}

public class UI_TreeConnectHandler : MonoBehaviour
{
    private RectTransform rect => GetComponent<RectTransform>();
    [SerializeField] private UI_TreeConnectDetails[] connectionDetails;//详细
    [SerializeField] private UI_TreeConnection[] connections;//连接

    private Image connectionImage;
    private Color originalColor;

    private void Awake()
    {
        if(connectionImage != null)
            originalColor = connectionImage.color;
    }

    // 编辑器模式下的验证方法
    private void OnValidate()
    {
        if(connectionDetails.Length <= 0)
            return;
        
        if (connectionDetails.Length != connections.Length)// 如果连接详细信息和连接点数量不一致，输出警告
        {
            Debug.Log("Amount of details should be same as amount of connections. - " + gameObject.name);
        }
        
        UpdateAllConnections();// 更新连接
    }

    // 更新所有连接
    private void UpdateConnections()
    {
        for (int i = 0; i < connectionDetails.Length; i++)// 遍历连接详细信息和连接点
        {
            var detail = connectionDetails[i];// 获取当前连接的详细信息
            var connection = connections[i]; // 获取当前连接点
            
            Vector2 targetPosition = connection.GetConnectionPoint(rect); // 获取连接点在屏幕上的目标位置
            Image connectionImage = connection.GetConnectionImage();
            
            connection.DirectConnection(detail.direction,detail.length,detail.rotation); // 设置连接的方向和长度

            if (detail.childNode == null)
                continue;
            
            detail.childNode.SetPosition(targetPosition); // 设置子节点的位置
            detail.childNode.SetConnectionImage(connectionImage);
            detail.childNode.transform.SetAsLastSibling();
        }
    }

    // 更新所有连接（包括当前连接和子节点的连接）
    public void UpdateAllConnections()
    {
        UpdateConnections();// 更新当前对象的连接

        foreach (var node in connectionDetails)// 遍历连接详细信息数组，更新每个子节点的连接
        {
            if (node.childNode == null) continue;// 如果子节点为空，则跳过
            node.childNode.UpdateConnections();// 更新子节点的连接
        }
    }

    // 解锁或锁定连接图像
    public void UnlockConnectionImage(bool unlocked)
    {
        if (connectionImage == null)// 如果 connectionImage 为空，直接返回
            return;

        connectionImage.color = unlocked ? Color.white : originalColor;// 如果 unlocked 为 true，则将颜色设置为白色，否则恢复原始颜色
    }

    // 设置连接图像
    public void SetConnectionImage(Image image) => connectionImage = image;
    
    // 设置UI元素的位置
    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;
}
