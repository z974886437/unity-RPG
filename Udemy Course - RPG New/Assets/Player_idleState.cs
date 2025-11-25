using UnityEngine;

public class Player_idleState : Player_GroundSedSt
{
    public Player_idleState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    
    public override void Update()
    {
        base.Update();
        
        if(player.moveInput.x != 0)// 检查玩家的水平方向输入是否不为零（即玩家有移动输入）
            stateMachine.ChangeState(player.moveState);// 如果玩家有水平输入，切换到移动状态
    }
}
