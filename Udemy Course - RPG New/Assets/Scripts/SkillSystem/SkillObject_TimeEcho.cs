using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkillObject_TimeEcho : SkillObject_Base
{

    [SerializeField] private float wispMoveSpeed = 15;
    [SerializeField] private GameObject onDeathVfx;
    [SerializeField] private LayerMask whatIsGround;
    private bool shouldMoveToPlayer;

    private Transform playerTransform;
    private Skill_TimeEcho echoManager;
    private TrailRenderer wispTrail;
    private Entity_Health playerHealth;
    private SkillObject_Health echoHealth;
    private Player_SkillManager skillManager;
    private Entity_StatusHandler statusHandler;
    
    public int maxAttacks { get; private set; }

    // 初始化时间回响对象，由技能管理器传入引用并控制生命周期
    public void SetupEcho(Skill_TimeEcho echoManager)
    {
        this.echoManager = echoManager;  // 缓存时间回响技能管理器，供后续逻辑使用
        playerStats = echoManager.player.stats;// 从技能管理器中获取玩家属性，用于伤害与治疗计算
        damageScaleData = echoManager.damageScaleData; // 缓存技能的伤害倍率数据，保证回响与技能数值一致
        maxAttacks = echoManager.GetMaxAttacks(); // 获取该回响允许的最大攻击次数，用于攻击终止判断
        playerTransform = echoManager.transform.root;// 获取玩家的根节点 Transform，作为幽魂返回的移动目标
        playerHealth = echoManager.player.health;// 缓存玩家生命组件，用于回响返回时执行治疗逻辑
        skillManager = echoManager.skillManager;// 缓存技能管理器，用于后续冷却缩减
        statusHandler = echoManager.player.statusHandler;// 缓存玩家状态处理器，用于清除负面效果
        

        Invoke(nameof(HandleDeath),echoManager.GetEchoDuration()); // 在回响持续时间结束后，自动触发死亡处理
        FlipToTarget();// 初始时根据目标方向翻转回响朝向，保证视觉正确
        
        echoHealth = GetComponent<SkillObject_Health>(); // 获取回响自身的生命组件，用于记录受到的伤害
        wispTrail = GetComponentInChildren<TrailRenderer>();// 获取幽魂拖尾特效组件，用于回响返回玩家时的表现
        wispTrail.gameObject.SetActive(false);// 初始状态下关闭幽魂特效，避免未死亡就显示
        
        anim.SetBool("canAttack",maxAttacks > 0);// 根据是否允许攻击来控制动画状态，防止播放无效攻击动画
    }
    
    // 每帧更新回响对象的动画与移动限制
    private void Update()
    {
        if(shouldMoveToPlayer) // 如果已经进入幽魂状态，则执行返回玩家的移动逻辑
            HandleWispMovement();
        else
        {
            anim.SetFloat("yVelocity",rb.linearVelocity.y);// 将刚体的垂直速度同步给动画参数，用于播放下落/落地动画
            StopHorizontalMovement();// 检测是否接触地面，并在需要时停止水平移动
        }
    }
    
    // 当幽魂接触玩家时触发的奖励逻辑
    private void HandlePlayerTouch()
    {
        float healAmount = echoHealth.lastDamageTaken * echoManager.GetPercentOfDamageHealed();// 根据回响最后一次受到的伤害计算治疗量，形成“伤害转化收益”
        playerHealth.IncreaseHealth(healAmount); // 为玩家恢复生命值，强化回响的资源回收价值

        float amountInSeconds = echoManager.GetCooldownReduceInSeconds();// 获取技能冷却缩减秒数，用于加快战斗节奏
        skillManager.ReduceAllSkillCooldownBy(amountInSeconds);// 为所有技能减少冷却时间，形成连招激励
        
        if(echoManager.CanRemoveNegativeEffects()) // 如果技能允许，则清除玩家身上的所有负面状态
            statusHandler.RemoveAllNegativeEffects();
    }

    // 控制幽魂向玩家移动并检测是否抵达
    private void HandleWispMovement()
    {
        // 使用 MoveTowards 保证幽魂以恒定速度追踪玩家位置
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, wispMoveSpeed * Time.deltaTime);

        // 当幽魂足够接近玩家时，视为成功回收
        if (Vector2.Distance(transform.position, playerTransform.position) < 0.5f)
        {
            HandlePlayerTouch();// 触发治疗、冷却缩减等奖励效果
            Destroy(gameObject);// 回收完成后销毁回响对象，结束生命周期
        }
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

        if (echoManager.ShouldBeWisp())// 根据管理器逻辑判断该回响是否应该转化为幽魂形态
            TurnIntoWisp();// 满足条件时进入幽魂状态，而不是直接销毁
        else
            Destroy(gameObject);// 销毁当前回响对象，结束其生命周期
    }

    // 将回响从“攻击实体”转化为“返回玩家的幽魂”
    private void TurnIntoWisp()
    {
        shouldMoveToPlayer = true; // 标记为需要向玩家移动，通常在 Update 中驱动位移逻辑
        anim.gameObject.SetActive(false);// 关闭原本的动画物体，防止继续播放攻击或死亡动画
        wispTrail.gameObject.SetActive(true);// 启用幽魂拖尾特效，强化“能量体返回”的视觉表现
        rb.simulated = false; // 关闭物理模拟，避免被碰撞或重力影响返回路径
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
