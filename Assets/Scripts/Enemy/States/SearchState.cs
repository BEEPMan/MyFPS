using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : BaseState
{
    private float searchTimer = 0f;

    public override void Enter()
    {
        stateMachine.Enemy.Agent.SetDestination(stateMachine.Enemy.Target.transform.position);
    }

    public override void Exit()
    {

    }

    public override void Perform()
    {
        searchTimer += Time.deltaTime;
        if(searchTimer > 3f)
            stateMachine.ChangeState(new PatrolState());
        if (stateMachine.Enemy.CanSeeTarget())
            stateMachine.ChangeState(new AttackState());
    }
}
