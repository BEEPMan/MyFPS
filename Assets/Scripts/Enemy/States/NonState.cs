using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonState : BaseState
{
    public override void Enter()
    {
        enemy.Agent.isStopped = true;
    }

    public override void Exit()
    {
        enemy.Agent.isStopped = false;
    }

    public override void Perform()
    {

    }
}
