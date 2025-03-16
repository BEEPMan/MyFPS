using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Unity.Netcode;

public class Shotgun : WeaponController
{
    protected override void Attack()
    {
        for (int i = 0; i < WeaponData.numOfShell; i++)
        {
            Vector2 shellDelta = Random.insideUnitCircle * WeaponData.scatterRate;
            Vector3 shellPosition = Camera.main.transform.position + Camera.main.transform.up * shellDelta.x + Camera.main.transform.right * shellDelta.y;
            shellPosition += Camera.main.transform.forward * WeaponData.range;
            Vector3 shellDirection = shellPosition - Camera.main.transform.position;
            Ray ray = new Ray(Camera.main.transform.position, shellDirection);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.rigidbody != null)
                    hitInfo.rigidbody.AddForce(Camera.main.transform.forward * 3.0f, ForceMode.Impulse);
                EnemyController target = hitInfo.transform.GetComponent<EnemyController>();
                if (target != null)
                {
                    if (NetworkManager.Singleton.IsServer)
                    {
                        int damage = CalculateDamage();
                        if (Utils.GetRandomResult(WeaponData.elementalProb))
                            target.TakeDamage(damage, WeaponData.elementalType, false);
                        else
                            target.TakeDamage(damage);
                    }
                }
                else if (hitInfo.transform.gameObject.activeSelf)
                {
                    CreateBulletHole(hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal)).Forget();
                }
            }
        }
    }
}