using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private float moveTimer;
    private float losePlayerTimer;
    private float fireTimer;

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
        if(enemy.CanSeePlayer())
        {
            losePlayerTimer = 0;
            moveTimer += Time.deltaTime;
            fireTimer += Time.deltaTime;
            Vector3 lookPos = enemy.Target.transform.position;
            lookPos.y = enemy.transform.position.y;
            enemy.transform.LookAt(lookPos);

            if(fireTimer > enemy.fireRate)
            {
                Fire();
            }
            if(moveTimer > Random.Range(3,7))
            {
                enemy.Agent.isStopped = false;
                enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5));
                moveTimer = 0;
            }
        }
        else
        {
            losePlayerTimer += Time.deltaTime;
            if(losePlayerTimer > 3f)
            {
                enemy.Agent.isStopped = false;
                stateMachine.ChangeState(new SearchState());
            }
        }
    }

    public void Fire()
    {
        Transform gunBarrel = enemy.gunBarrel;
        GameObject bullet = GameObject.Instantiate(Resources.Load("Prefabs/Bullet") as GameObject, gunBarrel.position, enemy.transform.rotation);
        bullet.transform.Rotate(new Vector3(90f, 0f, 0f), Space.Self);
        bullet.GetComponent<Bullet>().TargetTag = "Player";
        Vector3 fireDirection = (enemy.Target.transform.position - gunBarrel.transform.position).normalized;
        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(Random.Range(-3f, 3f), Vector3.up) * fireDirection * 40;
        fireTimer = 0;
    }
}
