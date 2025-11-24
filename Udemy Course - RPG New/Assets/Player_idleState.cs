using UnityEngine;

public class Player_idleState : EntityState
{
    public Player_idleState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    
    public override void Update()
    {
        base.Update();
        
        
        if(player.moveInput.x != 0)
            stateMachine.ChangeState(player.moveState);
    }
}
