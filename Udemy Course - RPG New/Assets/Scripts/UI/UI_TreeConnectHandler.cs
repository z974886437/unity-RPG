using System;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

[Serializable]
public class UI_TreeConnectDetails
{
    public UI_TreeConnectHandler childNode;
    public NodeDirectionType direction;
    [Range(100f, 350f)] public float length;
}

public class UI_TreeConnectHandler : MonoBehaviour
{
    private RectTransform rect;
    [SerializeField] private UI_TreeConnectDetails[] connectionDetails;//详细
    [SerializeField] private UI_TreeConnection[] connections;//连接

    // 编辑器模式下的验证方法
    private void OnValidate()
    {
        if (rect == null)// 如果 rect 为空，则获取当前物体的 RectTransform 组件
            rect = GetComponent<RectTransform>();
        
        if (connectionDetails.Length != connections.Length)// 如果连接详细信息和连接点数量不一致，输出警告
        {
            Debug.Log("Amount of details should be same as amount of connections. - " + gameObject.name);
        }
        
        UpdateConnections();// 更新连接
    }

    // 更新所有连接
    private void UpdateConnections()
    {
        for (int i = 0; i < connectionDetails.Length; i++)// 遍历连接详细信息和连接点
        {
            var detail = connectionDetails[i];// 获取当前连接的详细信息
            var connection = connections[i]; // 获取当前连接点
            Vector2 targetPosition = connection.GetConnectionPoint(rect); // 获取连接点在屏幕上的目标位置
            
            connection.DirectConnection(detail.direction,detail.length); // 设置连接的方向和长度
            detail.childNode.SetPosition(targetPosition); // 设置子节点的位置
        }
    }
    
    // 设置UI元素的位置
    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;
}
