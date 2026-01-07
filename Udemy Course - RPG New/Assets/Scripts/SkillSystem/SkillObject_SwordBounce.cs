using System.Collections.Generic;
using UnityEngine;

public class SkillObject_SwordBounce : SkillObject_Sword
{
    [SerializeField] private float bounceSpeed = 15;
    private int bounceCount;

    private Collider2D[] enemyTargets;
    private Transform nextTarget;
    private List<Transform> selectedBefore = new List<Transform>();//之前选择过


    // 重写父类的 SetupSword，用于初始化“弹射剑”的专属参数
    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        anim.SetTrigger("spin"); // 触发旋转动画，让剑在飞行中有视觉反馈
        base.SetupSword(swordManager, direction);

        bounceSpeed = swordManager.bounceSpeed;// 从技能管理器中读取弹射移动速度
        bounceCount = swordManager.bounceCount;// 从技能管理器中读取可弹射次数
    }

    // 每帧更新逻辑（剑的生命周期核心）
    protected override void Update()
    {
        HandleComeback(); // 处理剑回到玩家的逻辑（如时间、状态判断）
        HandleBounce();// 处理剑在敌人之间弹射的逻辑
    }

    // 处理剑向下一个目标弹射的移动与命中判断
    private void HandleBounce()
    {
        if (nextTarget == null)// 如果当前没有目标，直接退出，避免空引用
            return;

        // 使用 MoveTowards 让剑平滑地朝目标移动
        transform.position = Vector2.MoveTowards(transform.position, nextTarget.position, bounceSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, nextTarget.position) < 0.75f)// 判断是否接近目标（而不是完全重合，提升容错）
        {
            DamageEnemiesInRadius(transform, 1);  // 对目标附近敌人造成范围伤害
            BounceToNextTarget(); // 选择下一个弹射目标

            if (bounceCount == 0 || nextTarget == null) // 如果弹射次数用完，或已经没有可用目标
            {
                nextTarget = null;// 清空目标，防止后续 Update 继续执行弹射
                GetSwordBackToPlayer();// 强制让剑回到玩家手中
            }
        }

    }

    // 选择下一个弹射目标，并消耗一次弹射次数
    private void BounceToNextTarget()
    {
        nextTarget = GetNextTarget(); // 获取下一个合法目标（未被选中过的敌人）
        bounceCount--; // 每弹射一次，减少剩余次数
    }
    
    // 当剑第一次命中敌人时触发（进入弹射状态）
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyTargets == null)// 如果这是第一次命中敌人
        {
            enemyTargets = GetEnemiesAround(transform, 10); // 获取一定范围内的所有敌人作为弹射候选
            rb.simulated = false;// 关闭物理模拟，防止后续被碰撞影响轨迹
        }
        
        DamageEnemiesInRadius(transform,1); // 首次命中时立即造成一次范围伤害
        
        if(enemyTargets.Length <= 1 || bounceCount == 0) // 如果只有一个敌人，或没有弹射次数
            GetSwordBackToPlayer(); // 直接回收剑
        else
            nextTarget = GetNextTarget();// 否则进入弹射流程，选择下一个目标
    }

    // 获取下一个随机弹射目标
    private Transform GetNextTarget()
    {
        List<Transform> validTarget = GetValidTargets();// 获取当前所有可用目标（未选中过的）

        int randomIndex = Random.Range(0, validTarget.Count);// 从可用目标中随机选择一个

        Transform nextTarget = validTarget[randomIndex];// 获取被选中的目标
        selectedBefore.Add(nextTarget); // 记录该目标，避免重复弹射

        return nextTarget;// 返回该目标
    }

    // 获取当前“可弹射”的目标列表
    private List<Transform> GetValidTargets()
    {
        List<Transform> validTargets = new List<Transform>();// 存放最终可用的目标
        List<Transform> aliveTargets = GetAliveTargets();// 获取所有仍然存活的敌人

        foreach (var enemy in aliveTargets)// 遍历所有存活敌人
        {
            if(enemy != null && selectedBefore.Contains(enemy.transform) == false) // 排除已经被弹射过的敌人
                validTargets.Add(enemy.transform);
        }
        
        if(validTargets.Count > 0)// 如果还有未选中过的目标
            return validTargets;
        else
        {
            selectedBefore.Clear(); // 如果全部都选过了，清空记录，允许重新弹射
            return aliveTargets;// 返回所有存活敌人作为备选
        }
    }

    // 获取仍然存活的敌人 Transform 列表
    private List<Transform> GetAliveTargets()
    {
        List<Transform> aliveTargets = new List<Transform>();// 存放存活敌人的列表

        foreach (var enemy in enemyTargets)// 遍历最初记录的敌人数组
        {
            if(enemy != null) // 如果敌人未被销毁
                aliveTargets.Add(enemy.transform);
        }
        return aliveTargets;// 返回存活敌人列表
    }

    
}
