using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory_Weapon : UI_PopUp
{
    [Header("Ammo")]
    [SerializeField] private Image normalAmmoFront;
    [SerializeField] private TextMeshProUGUI normalAmmoText;
    [SerializeField] private Image largeAmmoFront;
    [SerializeField] private TextMeshProUGUI largeAmmoText;
    [SerializeField] private Image specialAmmoFront;
    [SerializeField] private TextMeshProUGUI specialAmmoText;

    [Header("WeaponDetails")]
    [SerializeField] private Image[] weaponIcons = new Image[3];
    [SerializeField] private TextMeshProUGUI[] weaponNames = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] weaponDescriptions = new TextMeshProUGUI[3];

    //private bool isOpenFirstTime = true;

    public override void OnPopUp()
    {
        UpdateAmmoFillAmount();
        UpdateWeaponDetails();
    }

    public void UpdateAmmoFillAmount()
    {
        int normalRemainAmmo = GameManager.Instance.Player.Ammos[(int)EnumTypes.AmmoType.Normal];
        int normalMaxAmmo = GameManager.Instance.Player.MaxAmmos[(int)EnumTypes.AmmoType.Normal];
        int largeRemainAmmo = GameManager.Instance.Player.Ammos[(int)EnumTypes.AmmoType.Large];
        int largeMaxAmmo = GameManager.Instance.Player.MaxAmmos[(int)EnumTypes.AmmoType.Large];
        int specialRemainAmmo = GameManager.Instance.Player.Ammos[(int)EnumTypes.AmmoType.Special];
        int specialMaxAmmo = GameManager.Instance.Player.MaxAmmos[(int)EnumTypes.AmmoType.Special];
        normalAmmoFront.fillAmount = (float)normalRemainAmmo / normalMaxAmmo;
        largeAmmoFront.fillAmount = (float)largeRemainAmmo / largeMaxAmmo;
        specialAmmoFront.fillAmount = (float)specialRemainAmmo / specialMaxAmmo;
        normalAmmoText.text = string.Concat(normalRemainAmmo, "/", normalMaxAmmo);
        largeAmmoText.text = string.Concat(largeRemainAmmo, "/", largeMaxAmmo);
        specialAmmoText.text = string.Concat(specialRemainAmmo, "/", specialMaxAmmo);
    }

    public void UpdateWeaponDetails()
    {
        Weapon weapon;
        for (int i = 0; i < 3; i++)
        {
            weapon = GameManager.Instance.Player.Weapons[i].WeaponData;
            if (weapon == null) continue;
            Color color = weaponIcons[i].color;
            color.a = 1f;
            weaponIcons[i].color = color;
            weaponIcons[i].sprite = weapon.Icon;
            weaponNames[i].text = weapon.ItemName;
            weaponDescriptions[i].text = weapon.Description;
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
        UIManager.Instance.HidePanel("UI_Inventory_Weapon");
        UIManager.Instance.ShowPanel("UI_Inventory_Scroll");
    }
}
