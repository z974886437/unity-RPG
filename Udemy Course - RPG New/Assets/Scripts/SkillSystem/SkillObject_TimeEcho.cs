using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkillObject_TimeEcho : SkillObject_Base
{

    [SerializeField] private GameObject onDeathVfx;
    [SerializeField] private LayerMask whatIsGround;
    private Skill_TimeEcho echoManager;

    public int maxAttacks { get; private set; }

    // 初始化时间回响对象，由技能管理器传入引用并控制生命周期
    public void SetupEcho(Skill_TimeEcho echoManager)
    {
        this.echoManager = echoManager;  // 缓存时间回响技能管理器，供后续逻辑使用
        playerStats = echoManager.player.stats;
        damageScaleData = echoManager.damageScaleData;
        maxAttacks = echoManager.GetMaxAttacks();
        
        FlipToTarget();
        anim.SetBool("canAttack",maxAttacks > 0);
        Invoke(nameof(HandleDeath),echoManager.GetEchoDuration()); // 在回响持续时间结束后，自动触发死亡处理
    }
    
    // 每帧更新回响对象的动画与移动限制
    private void Update()
    {
        anim.SetFloat("yVelocity",rb.linearVelocity.y);// 将刚体的垂直速度同步给动画参数，用于播放下落/落地动画
        StopHorizontalMovement();// 检测是否接触地面，并在需要时停止水平移动
    }

    //翻转为目标
    private void FlipToTarget()
    {
        Transform target = FindClosestTarget();// 找到最近的目标
        
        if (target != null && target.position.x < transform.position.x) // 如果目标存在且在自己左边
            transform.Rotate(0,180,0); // 将自己绕 Y 轴旋转 180°，面向目标
    }

    //执行攻击
    public void PerformAttack()
    {
        DamageEnemiesInRadius(targetCheck,1);// 在目标检测点周围半径为1的范围内对敌人造成伤害
        
        if(targetGotHit == false) // 如果没有命中目标就直接返回，不继续执行后续逻辑
            return;
        
        bool canDuplicate = Random.value < echoManager.GetDuplicateChance();// 计算是否可以产生复制体（随机概率判断）
        float xOffset = transform.position.x < lastTarget.position.x ? 1 : -1; // 根据自己和上一个目标的 X 位置，决定复制体偏移方向
        
        if(canDuplicate) // 如果满足复制条件，则在上一个目标位置创建时间回响
            echoManager.CreateTimeEcho(lastTarget.position + new Vector3(xOffset,0));
    }

    // 回响结束或被销毁时的统一死亡处理
    public void HandleDeath()
    {
        Instantiate(onDeathVfx, transform.position, Quaternion.identity);// 在当前位置生成死亡特效，增强反馈表现
        Destroy(gameObject);// 销毁当前回响对象，结束其生命周期
    }

    // 当回响落地后，强制停止其水平方向的移动
    private void StopHorizontalMovement()
    {
        // 向下发射射线，用于检测回响是否接触地面
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, whatIsGround);
        
        if(hit.collider != null)// 如果检测到地面，清空水平速度，仅保留垂直速度
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }
}
