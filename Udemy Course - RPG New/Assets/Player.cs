using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public StateMachine stateMachine { get; private set; }
    private StateMachine stateMachine;
    
    public Player_idleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }

    private void Awake()
    {
        stateMachine = new StateMachine();

        idleState = new Player_idleState(this,stateMachine, "Idle");
        moveState = new Player_MoveState(this,stateMachine, "move");
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        stateMachine.currentState.Update();
    }
}
