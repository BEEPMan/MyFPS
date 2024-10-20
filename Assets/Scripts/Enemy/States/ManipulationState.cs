using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulationState : BaseState
{
    private float fireTimer;

    public override void Enter()
    {
        enemy.Agent.stoppingDistance = 5.0f;
    }

    public override void Exit()
    {
        enemy.Agent.stoppingDistance = 0.1f;
        enemy.Target = Player.Instance.gameObject;
        //enemy.Target = null;
    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer())
        {
            fireTimer += Time.deltaTime;
            enemy.transform.LookAt(enemy.Target.transform);

            if (fireTimer > enemy.fireRate)
            {
                Fire();
            }
        }
    }

    public void Fire()
    {
        Transform gunBarrel = enemy.gunBarrel;
        GameObject bullet = GameObject.Instantiate(Resources.Load("Prefabs/Bullet") as GameObject, gunBarrel.position, enemy.transform.rotation);
        bullet.GetComponent<Bullet>().TargetTag = "Enemy";
        Vector3 fireDirection = (enemy.Target.transform.position - gunBarrel.transform.position).normalized;
        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(Random.Range(-3f, 3f), Vector3.up) * fireDirection * 40;
        fireTimer = 0;
    }
}
