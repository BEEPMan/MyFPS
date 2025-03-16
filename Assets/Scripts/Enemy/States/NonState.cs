using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonState : BaseState
{


    public override void Enter()
    {
        stateMachine.Enemy.Agent.enabled = false;
    }

    public override void Exit()
    {
        stateMachine.Enemy.Agent.enabled = true;
    }

    public override void Perform()
    {

    }
}
