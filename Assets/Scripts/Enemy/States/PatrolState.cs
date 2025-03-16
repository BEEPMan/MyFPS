using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : BaseState
{
    public int waypointIndex;
    public float waitTimer;

    public override void Enter()
    {
        
    }

    public override void Exit()
    {

    }

    public override void Perform()
    {
        PatrolCycle();
        if (stateMachine.Enemy.CanSeeTarget())
        {
            stateMachine.ChangeState(new AttackState());
        }
    }

    public void PatrolCycle()
    {
        if(stateMachine.Enemy.Agent.remainingDistance < 1f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer > 3f)
            {
                if (waypointIndex < stateMachine.Enemy.path.waypoints.Count - 1)
                    waypointIndex++;
                else
                    waypointIndex = 0;
                stateMachine.Enemy.Agent.SetDestination(stateMachine.Enemy.path.waypoints[waypointIndex].position);
                waitTimer = 0;
            }
        }
    }
}
