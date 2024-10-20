using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    public override void Fire()
    {
        if (RemainAmmo <= 0 || fireTimer > 0 || reloadTimer > 0)
        {
            return;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, range))
        {
            Stat target;
            if (target = hitInfo.transform.GetComponent<Stat>())
            {
                float damage = Player.Instance.PStat.damageCalculator.CalculateGivenDamage(bulletDamage, elementalType, true, false);
                target.TakeDamage(damage, elementalType, Player.Instance.PStat);
            }
            else if (hitInfo.transform.gameObject.activeSelf)
            {
                StartCoroutine(CreateBulletHole(hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal)));
            }
        }

        RecoilFire();
        RemainAmmo--;
        Player.Instance.UI.SetAmmoText(ammoType, RemainAmmo, Player.Instance.PWeapon.ammo.Check(ammoType));
        if (RemainAmmo <= 0)
        {
            Reload();
        }
        else
            fireTimer = Time.deltaTime;
    }

    public override void Skill()
    {

    }
}
