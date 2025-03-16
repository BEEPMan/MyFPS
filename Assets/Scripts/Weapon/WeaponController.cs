using Cysharp.Threading.Tasks;
using EnumTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public PlayerController Player { get; protected set; }
    public Weapon WeaponData { get; protected set; }
    public int RemainAmmo { get; protected set; }

    #region Flag
    protected bool isFirstInit = true;
    [HideInInspector] public bool isReloading = false;
    [HideInInspector] public bool isAttacking = false;
    #endregion

    public void Init(PlayerController player, Weapon weapon = null)
    {
        Player = player;
        if (weapon == null)
            WeaponData = GetComponent<WeaponPickUp>().weapon;
        else
            WeaponData = weapon;
        if (isFirstInit)
        {
            RemainAmmo = WeaponData.ammoCapacity;
            isFirstInit = false;
        }
    }
    public async UniTaskVoid TryAttack()
    {
        if (RemainAmmo <= 0 || isAttacking || isReloading)
        {
            return;
        }
        isAttacking = true;
        Attack();
        Player.Look.Recoil(WeaponData.recoilRate);
        RemainAmmo--;
        if (Player.NetworkObjectId == GameManager.Instance.PlayerID)
            UIManager.Instance.InGame.UpdateAmmoText(WeaponData.ammoType, RemainAmmo, Player.Ammos[(int)WeaponData.ammoType]);
        await UniTask.Delay(TimeSpan.FromSeconds(1f / WeaponData.fireRate));
        isAttacking = false;
        if (RemainAmmo <= 0)
        {
            Player.Reload();
        }
    }
    protected virtual void Attack()
    {
        Ray ray = new Ray(Player.Hand.position, Player.Hand.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, WeaponData.range))
        {
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
    public virtual void Skill() { }
    public async UniTaskVoid Reload()
    {
        if (WeaponData.ammoCapacity <= 0 || WeaponData.ammoCapacity == RemainAmmo || isReloading)
            return;
        if (WeaponData.ammoType != EnumTypes.AmmoType.Infinite && Player.Ammos[(int)WeaponData.ammoType] == 0)
            return;

        isReloading = true;
        await UniTask.Delay(TimeSpan.FromSeconds(WeaponData.reloadTime));

        if (WeaponData.ammoType == EnumTypes.AmmoType.Infinite)
            RemainAmmo = WeaponData.ammoCapacity;
        else
        {
            int ammoNeed = WeaponData.ammoCapacity - RemainAmmo;
            if (Player.Ammos[(int)WeaponData.ammoType] < ammoNeed)
            {
                Player.Ammos[(int)WeaponData.ammoType] = 0;
                RemainAmmo += Player.Ammos[(int)WeaponData.ammoType];
            }
            else
            {
                Player.Ammos[(int)WeaponData.ammoType] -= ammoNeed;
                RemainAmmo += ammoNeed;
            }
        }
        if(Player.NetworkObjectId == GameManager.Instance.PlayerID)
        {
            UIManager.Instance.InGame.UpdateAmmoText(WeaponData.ammoType, RemainAmmo, Player.Ammos[(int)WeaponData.ammoType]);
            UIManager.Instance.InGame.UpdateAmmoFillAmount(WeaponData.ammoType, Player.Ammos[(int)WeaponData.ammoType], Player.MaxAmmos[(int)WeaponData.ammoType]);
        }
        isReloading = false;
    }

    protected int CalculateDamage()
    {
        int finalDamage = (int)(WeaponData.damage * (100 + Player.WeaponDamage + Player.ElementalDamage[(int)WeaponData.elementalType]) / 100);
        return (int)(finalDamage * Player.FinalDamage);
    }

    protected async UniTaskVoid CreateBulletHole(Vector3 position, Quaternion rotation)
    {
        GameObject bulletHoleGO = ObjectPool.Instance.Pop("BulletHole", position, rotation);
        await UniTask.Delay(TimeSpan.FromSeconds(5.0f));
        ObjectPool.Instance.Push(bulletHoleGO);
    }
}
