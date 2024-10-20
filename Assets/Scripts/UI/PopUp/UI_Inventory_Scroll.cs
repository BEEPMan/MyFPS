using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inventory_Scroll : UI_PopUp
{
    [Header("Scrolls")]
    public GameObject scrolls;
    private List<Image> scrollIcons = new();

    [Header("Scroll Details")]
    public Image scrollDetailIcon;
    public TextMeshProUGUI scrollDetailName;
    public TextMeshProUGUI scrollDetailDescription;

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

    private bool isOpenFirstTime = true;

    protected override void Init()
    {
        InitScrollInventory();
    }

    private void Update()
    {
        
    }

    public override void OnPopUp()
    {
        if (isOpenFirstTime)
        {
            InitScrollInventory();
            isOpenFirstTime = false;
        }
        ClearScrollDetail();
        SetScrollInventory();
        //SetAmmoFillAmount();
        //SetWeaponDetails();
    }

    public void InitScrollInventory()
    {
        foreach(Image item in scrolls.transform.GetComponentsInChildren<Image>())
        {
            if(item.name == "Icon")
            {
                scrollIcons.Add(item);
            }
        }
    }

    public void SetScrollInventory()
    {
        for (int i = 0; i < Player.Instance.PStat.scrolls.Count; i++)
        {
            Color color = scrollIcons[i].color;
            color.a = 1f;
            scrollIcons[i].color = color;
            scrollIcons[i].sprite = Player.Instance.PStat.scrolls[i].scrollIcon;
            if (scrollIcons[i].GetComponent<UI_Inventory_Item>() != null)
            {
                Destroy(scrollIcons[i].GetComponent<UI_Inventory_Item>());
            }
            scrollIcons[i].AddComponent<UI_Inventory_Item>().scroll = Player.Instance.PStat.scrolls[i];
        }
    }

    public void SetScrollDetail(Scroll scroll)
    {
        Color color = scrollDetailIcon.color;
        color.a = 1f;
        scrollDetailIcon.color = color;
        scrollDetailIcon.sprite = scroll.scrollIcon;
        scrollDetailName.text = scroll.name;
        scrollDetailDescription.text = scroll.description;
    }

    public void ClearScrollDetail()
    {
        Color color = scrollDetailIcon.color;
        color.a = 0f;
        scrollDetailIcon.color = color;
        scrollDetailName.text = "";
        scrollDetailDescription.text = "";
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
        largeAmmoFront.fillAmount = (float)largeRemainAmmo/largeMaxAmmo;
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
        UIManager.Instance.OpenPopUp(PopUpType.Weapon);
    }
}
