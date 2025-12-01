using UnityEngine;

public class Enemy_GroundedState : EnemyState
{
    public Enemy_GroundedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();
        
        if(enemy.PlayerDetected() == true)// 检测敌人是否发现玩家
            stateMachine.ChangeState(enemy.battleState);// 如果检测到玩家，切换到敌人的战斗状态
    }
}
