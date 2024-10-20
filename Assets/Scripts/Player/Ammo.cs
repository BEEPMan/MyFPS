using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmmoType
{
    Infinite,
    Normal,
    Large,
    Special,
}

public class Ammo
{
    public List<int> maxAmmo = new();
    public List<int> remainAmmo = new();

    public AmmoType currentAmmoType;
    public Weapon currentWeapon;

    public void Init()
    {
        maxAmmo.Add(0);
        maxAmmo.Add(450);
        maxAmmo.Add(120);
        maxAmmo.Add(30);
        remainAmmo = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            remainAmmo.Add(maxAmmo[i] / 2);
            Player.Instance.UI.SetAmmoFillAmount((AmmoType)i, remainAmmo[i], maxAmmo[i]);
        }
    }

    public int Supply(AmmoType ammoType, int amount)
    {
        int overAmmo = remainAmmo[(int)ammoType] + amount - maxAmmo[(int)ammoType];
        int retValue = amount;
        if (overAmmo > 0)
        {
            remainAmmo[(int)ammoType] = maxAmmo[(int)ammoType];
            retValue = amount - overAmmo;
        }
        else
            remainAmmo[(int)ammoType] = remainAmmo[(int)ammoType] + amount;
        Player.Instance.UI.SetAmmoFillAmount(ammoType, remainAmmo[(int)ammoType], maxAmmo[(int)ammoType]);
        if (ammoType == currentAmmoType)
            Player.Instance.UI.SetAmmoText(ammoType, currentWeapon.RemainAmmo, remainAmmo[(int)ammoType]);
        return retValue;
    }

    public int Demand(AmmoType ammoType, int amount)
    {
        int underAmmo = -(remainAmmo[(int)ammoType] - amount);
        int retValue = amount;
        if (underAmmo > 0)
        {
            remainAmmo[(int)ammoType] = 0;
            retValue = amount - underAmmo;
        }
        else
            remainAmmo[(int)ammoType] = remainAmmo[(int)ammoType] - amount;
        Player.Instance.UI.SetAmmoFillAmount(ammoType, remainAmmo[(int)ammoType], maxAmmo[(int)ammoType]);
        if (ammoType == currentAmmoType)
            Player.Instance.UI.SetAmmoText(ammoType, currentWeapon.RemainAmmo, remainAmmo[(int)ammoType]);
        return retValue;
    }

    public int Reload(AmmoType ammoType, int currentBullet, int maxBullet)
    {
        if(ammoType == AmmoType.Infinite)
        {
            Player.Instance.UI.SetAmmoText(ammoType, maxBullet, 0);
            return maxBullet - currentBullet;
        }

        int remains;
        if (remainAmmo[(int)ammoType] >= maxBullet - currentBullet)
        {
            remains = maxBullet - currentBullet;
            remainAmmo[(int)ammoType] -= maxBullet - currentBullet;
        }
        else
        {
            remains = remainAmmo[(int)ammoType];
            remainAmmo[(int)ammoType] = 0;
        }
        Player.Instance.UI.SetAmmoFillAmount(ammoType, remainAmmo[(int)ammoType], maxAmmo[(int)ammoType]);
        Player.Instance.UI.SetAmmoText(ammoType, currentBullet + remains, remainAmmo[(int)ammoType]);
        return remains;
    }

    public int Check(AmmoType ammoType)
    {
        if (ammoType == AmmoType.Infinite) return 0;
        return remainAmmo[(int)ammoType];
    }
}
