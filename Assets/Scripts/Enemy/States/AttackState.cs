using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private float moveTimer;
    private float loseTargetTimer;
    private float fireTimer;

    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void Perform()
    {
        Vector3 lookPos = stateMachine.Enemy.Target.transform.position;
        lookPos.y = stateMachine.Enemy.transform.position.y;
        stateMachine.Enemy.transform.LookAt(lookPos);
        if (stateMachine.Enemy.CanSeeTarget())
        {
            loseTargetTimer = 0f;

            fireTimer += Time.deltaTime;
            if (fireTimer > stateMachine.Enemy.fireRate)
            {
                stateMachine.Enemy.Attack();
                fireTimer = 0;
            }

            float distanceToPlayer = Vector3.Distance(stateMachine.Enemy.transform.position, stateMachine.Enemy.Target.transform.position);
            if (distanceToPlayer > 6f)
            {
                stateMachine.Enemy.Agent.SetDestination(stateMachine.Enemy.Target.transform.position);
            }
            else if (distanceToPlayer < 4f)
            {
                Vector3 backDirection = (stateMachine.Enemy.transform.position - stateMachine.Enemy.Target.transform.position).normalized;
                Vector3 destination = stateMachine.Enemy.Target.transform.position + backDirection * 5f;
                stateMachine.Enemy.Agent.SetDestination(destination);
            }
            else
            {
                //stateMachine.Enemy.Agent.ResetPath();
            }
        }
        else
        {
            loseTargetTimer += Time.deltaTime;
            if(loseTargetTimer > 3f)
            {
                stateMachine.ChangeState(new SearchState());
            }
        }
    }
}
