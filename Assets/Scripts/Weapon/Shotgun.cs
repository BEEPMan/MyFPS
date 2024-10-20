using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Shotgun : Weapon
{
    [SerializeField] protected float firePower;
    [SerializeField] protected int numOfShell;
    [SerializeField] protected float scatterRate;

    public override void Fire()
    {
        if (RemainAmmo <= 0 || fireTimer > 0 || reloadTimer > 0)
        {
            return;
        }

        for (int i = 0; i < numOfShell; i++)
        {
            Vector2 shellDelta = Random.insideUnitCircle * scatterRate;
            Vector3 shellPosition = cam.transform.position + cam.transform.up * shellDelta.x + cam.transform.right * shellDelta.y;
            shellPosition += cam.transform.forward * range;
            Vector3 shellDirection = shellPosition - cam.transform.position;
            Ray ray = new Ray(cam.transform.position, shellDirection);
            Debug.DrawRay(cam.transform.position, shellDirection, Color.red, 2f);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.rigidbody != null)
                    hitInfo.rigidbody.AddForce(cam.transform.forward * firePower, ForceMode.Impulse);
                EnemyStat target;
                if (target = hitInfo.transform.GetComponent<EnemyStat>())
                {
                    float damage = Player.Instance.PStat.damageCalculator.CalculateGivenDamage(bulletDamage, elementalType, true, false);
                    target.TakeDamage(damage, elementalType, Player.Instance.PStat);
                    target.AddBuff(Player.Instance.PStat, "Stagger", 0.5f, 0f, cam.transform.forward * firePower);
                }
                else if (hitInfo.transform.gameObject.activeSelf)
                {
                    StartCoroutine(CreateBulletHole(hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal)));
                }
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

    public override string GetDescription()
    {
        StringBuilder sbDescription = new StringBuilder();
        sbDescription.Append("사거리: ");
        sbDescription.Append(range);
        sbDescription.Append("\n대미지: ");
        sbDescription.Append(bulletDamage);
        sbDescription.Append(" x ");
        sbDescription.Append(numOfShell);
        sbDescription.Append("\n탄창용량: ");
        sbDescription.Append(maxAmmo);
        sbDescription.Append("\n초당 발사량: ");
        sbDescription.Append(fireRate);
        return sbDescription.ToString();
    }
}
