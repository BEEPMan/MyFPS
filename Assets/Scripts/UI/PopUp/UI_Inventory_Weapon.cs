using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory_Weapon : UI_PopUp
{
    [Header("Ammo")]
    public Image normalAmmoFront;
    public TextMeshProUGUI normalAmmoText;
    public Image largeAmmoFront;
    public TextMeshProUGUI largeAmmoText;
    public Image specialAmmoFront;
    public TextMeshProUGUI specialAmmoText;

    [Header("WeaponDetails")]
    public Image[] weaponIcons = new Image[3];
    public TextMeshProUGUI[] weaponNames = new TextMeshProUGUI[3];
    public TextMeshProUGUI[] weaponDescriptions = new TextMeshProUGUI[3];

    //private bool isOpenFirstTime = true;

    public override void OnPopUp()
    {
        SetAmmoFillAmount();
        SetWeaponDetails();
    }

    public void SetAmmoFillAmount()
    {
        int normalRemainAmmo = Player.Instance.PWeapon.ammo.remainAmmo[(int)AmmoType.Normal];
        int normalMaxAmmo = Player.Instance.PWeapon.ammo.maxAmmo[(int)AmmoType.Normal];
        int largeRemainAmmo = Player.Instance.PWeapon.ammo.remainAmmo[(int)AmmoType.Large];
        int largeMaxAmmo = Player.Instance.PWeapon.ammo.maxAmmo[(int)AmmoType.Large];
        int specialRemainAmmo = Player.Instance.PWeapon.ammo.remainAmmo[(int)AmmoType.Special];
        int specialMaxAmmo = Player.Instance.PWeapon.ammo.maxAmmo[(int)AmmoType.Special];
        normalAmmoFront.fillAmount = (float)normalRemainAmmo / normalMaxAmmo;
        largeAmmoFront.fillAmount = (float)largeRemainAmmo / largeMaxAmmo;
        specialAmmoFront.fillAmount = (float)specialRemainAmmo / specialMaxAmmo;
        normalAmmoText.text = string.Concat(normalRemainAmmo, "/", normalMaxAmmo);
        largeAmmoText.text = string.Concat(largeRemainAmmo, "/", largeMaxAmmo);
        specialAmmoText.text = string.Concat(specialRemainAmmo, "/", specialMaxAmmo);
    }

    public void SetWeaponDetails()
    {
        Weapon weapon;
        for (int i = 0; i < 3; i++)
        {
            weapon = Player.Instance.PWeapon.GetWeapon(i + 1);
            if (weapon == null) continue;
            Color color = weaponIcons[i].color;
            color.a = 1f;
            weaponIcons[i].color = color;
            weaponIcons[i].sprite = weapon.weaponIcon;
            weaponNames[i].text = weapon.name;
            weaponDescriptions[i].text = weapon.GetDescription();
        }
    }

    public void ClearWeaponDetails()
    {
        for (int i = 0; i < 3; i++)
        {
            Color color = weaponIcons[i].color;
            color.a = 0f;
            weaponIcons[i].color = color;
            weaponNames[i].text = "";
            weaponDescriptions[i].text = "";
        }
    }

    public void ChangeTab()
    {
        UIManager.Instance.ClosePopUp();
        UIManager.Instance.OpenPopUp(PopUpType.Scroll);
    }
}
