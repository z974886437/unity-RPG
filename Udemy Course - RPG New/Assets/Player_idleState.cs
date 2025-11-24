using UnityEngine;

public class Player_idleState : EntityState
{
    public Player_idleState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }
    
    public override void Update()
    {
        base.Update();
        
        if(Input.GetKeyDown(KeyCode.F))
            stateMachine.ChangeState(player.moveState);
    }
}
