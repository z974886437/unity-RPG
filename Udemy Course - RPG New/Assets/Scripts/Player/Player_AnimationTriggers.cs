using UnityEngine;

public class Player_AnimationTriggers : Entity_AnimationTriggers
{
    private Player player;


    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Player>(); // 从当前对象的父物体中获取 Player 组件，用于后续访问玩家相关功能
    }

    // 私有方法：封装“丢剑”行为，作为对外调用的统一入口
    private void ThrowSword() => player.skillManager.swordThrow.ThrowSword();// 通过玩家 → 技能管理器 → 剑投掷技能，触发实际的丢剑逻辑
}
