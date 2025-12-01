using UnityEngine;

public class Enemy_AnimationTriggers : Entity_AnimationTriggers
{
    private Enemy enemy;
    private Enemy_VFX enemyVfx;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponentInParent<Enemy>();// 获取父物体上的 Enemy 组件
        enemyVfx = GetComponentInParent<Enemy_VFX>();// 获取父物体上的 Enemy_VFX 组件
    }

    // 启用反击窗口
    private void EnableCounterWindow()
    {
        enemyVfx.EnableAttackAlert(true);// 启用攻击警报特效
        enemy.EnableCounterWindow(true);// 启用反击窗口
    }

    // 禁用反击窗口
    private void DisableCounterWindow()
    {
        enemyVfx.EnableAttackAlert(false);// 禁用攻击警报特效
        enemy.EnableCounterWindow(false);// 禁用反击窗口
    }
}
