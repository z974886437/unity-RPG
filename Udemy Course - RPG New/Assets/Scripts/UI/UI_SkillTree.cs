using UnityEngine;

public class UI_SkillTree : MonoBehaviour
{
    [SerializeField] private int skillPoints;
    [SerializeField] private UI_TreeConnectHandler[] parentNodes;

    private void Start()
    {
        UpdateAllConnections();
    }

    // 通过右键点击 Unity 编辑器中的组件来调用此方法（ContextMenu 提供快捷菜单）
    [ContextMenu("Reset Skill Tree")]
    public void RefundAllSkills()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();// 获取当前对象及其子物体中的所有 UI_TreeNode 组件（这些节点通常代表技能节点）

        foreach (var node in skillNodes) // 遍历所有技能节点，并对每个节点调用 Refund 方法
            node.Refund();// 调用 Refund 方法进行技能退款
    }
    
    // 判断是否有足够的技能点
    public bool EnoughSkillPoints(int cost) => skillPoints >= cost;
    
    // 移除技能点
    public void RemoveSkillPoints(int cost) => skillPoints = skillPoints - cost;

    // 添加指定数量的技能点
    public void AddSkillPoints(int cost) => skillPoints = skillPoints + cost;
    
    
    // 通过右键点击 Unity 编辑器中的组件来调用此方法（ContextMenu 提供快捷菜单）
    [ContextMenu("Update All Connections")]
    public void UpdateAllConnections()
    {
        foreach (var node in parentNodes)// 遍历所有父节点，更新每个节点的连接
        {
            node.UpdateAllConnections(); // 调用每个父节点的 UpdateAllConnections 方法
        }
    }
}
