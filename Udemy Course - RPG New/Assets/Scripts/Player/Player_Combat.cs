using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("Counter attack details")]
    [SerializeField] private float counterRecovery = 0.1f;
        
    // 执行反击攻击并返回是否成功
    public bool CounterAttackPerformed()
    {
        
        bool hasPerformedCounter = false; // 标记是否执行了反击攻击
        
        foreach (var target in GetDetectedColliders()) // 遍历所有检测到的碰撞体
        {
            ICounterable counterable = target.GetComponent<ICounterable>();// 尝试获取该物体是否可以被反击（通过 ICounterable 接口）

            if (counterable == null)// 如果物体不支持反击，跳过
                continue;

            if (counterable.CanBeCountered)// 如果物体可以被反击
            {
                counterable.HandleCounter();// 处理反击
                hasPerformedCounter = true; // 标记已执行反击
            }
        }
        
        return hasPerformedCounter; // 返回是否成功执行反击
    }
    
    // 获取反击恢复时间
    public float GetCounterRecoveryDuration() => counterRecovery; // 返回反击恢复时间
}
