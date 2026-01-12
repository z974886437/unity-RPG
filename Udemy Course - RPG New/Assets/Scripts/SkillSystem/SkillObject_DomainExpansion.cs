using System;
using UnityEngine;

public class SkillObject_DomainExpansion : SkillObject_Base
{
    private Skill_DomainExpansion domainManager;
    
    private float expandSpeed = 2;
    private float duration;
    private float slowDownPercent = 0.9f;

    private Vector3 targetScale;
    private bool isShrinking;


    // 设置领域的核心数据，由技能管理器在生成时调用
    public void SetupDomain(Skill_DomainExpansion domainManager)
    {
        this.domainManager = domainManager; // 缓存技能管理器引用，方便后续使用技能参数

        duration = domainManager.GetDomainDuration();// 从技能管理器获取领域持续时间，决定多久后开始收缩
        slowDownPercent = domainManager.GetSlowPercentage();// 获取减速百分比，用于影响敌人移动速度
        expandSpeed = domainManager.expandSpeed;// 获取扩展速度，用于控制 Lerp 插值快慢
        float maxSize = domainManager.maxDomainSize;// 获取领域允许的最大尺寸
        
        targetScale = Vector3.one * maxSize;// 将目标缩放设置为最大尺寸（Vector3.one 统一 XYZ）
        Invoke(nameof(ShrinkDomain),duration);// 在持续时间结束后调用 ShrinkDomain，开始收缩领域
    }

    // Unity 每帧调用，用于持续更新领域状态
    private void Update()
    {
        HandleScaling();// 每一帧都处理缩放逻辑
    }

    // 负责处理领域的放大与缩小动画
    private void HandleScaling()
    {
        float sizeDifference = Mathf.Abs(transform.localScale.x - targetScale.x); // 计算当前缩放值与目标缩放值的差距（只看 X 轴即可）
        bool shouldChangeScale = sizeDifference > 0.1f;// 当差距大于阈值时，才需要继续缩放，避免抖动

        if (shouldChangeScale) // 如果还没接近目标大小，则使用 Lerp 平滑缩放
            transform.localScale = Vector3.Lerp(
                transform.localScale, // 起点：当前缩放值
                targetScale, // 终点：目标缩放值
                expandSpeed * Time.deltaTime);// 插值速度：速度 * deltaTime，保证帧率无关
        
        // 如果正在收缩，且已经非常接近 0，则销毁该领域对象
        if (isShrinking && sizeDifference < 0.1f)
            TerminateDomain();
        
    }

    // 终止领域技能，并清理所有领域相关状态
    private void TerminateDomain()
    {
        domainManager.ClearTargets();// 清空领域中记录的所有目标
        Destroy(gameObject);// 销毁领域物体本身
    }

    // 在领域持续时间结束后调用，用于开始收缩领域
    private void ShrinkDomain()
    {
        targetScale = Vector3.zero; // 将目标缩放设置为 0，表示完全消失
        isShrinking = true; // 标记当前状态为正在收缩，用于销毁判断
    }

    // 当敌人第一次进入领域范围时触发
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();// 尝试从碰撞体上获取 Enemy 组件

        if (enemy == null) // 如果不是敌人，则直接返回，避免空引用
            return;
        
        domainManager.AddTarget(enemy);
        enemy.SlowDownEntity(duration,slowDownPercent,true);// 对敌人施加减速效果，并标记为领域减速
    }

    // 当敌人离开领域范围时触发
    private void OnTriggerExit2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>(); // 尝试获取敌人组件

        if (enemy == null) // 如果不是敌人，直接返回
            return;
        
        enemy.StopSlowDown(); // 敌人离开领域后，立即取消减速效果
    }
}
