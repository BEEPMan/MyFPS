using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;

public class UI_InGame : MonoBehaviour
{
    [Header("Prompt Text")]
    [SerializeField] private TextMeshProUGUI promptText;

    [Header("HP Bar")]
    [SerializeField] private Image frontHPBar;
    [SerializeField] private Image backHPBar;
    [SerializeField] private TextMeshProUGUI HPText;

    [Header("Shield/Armor Bar")]
    [SerializeField] private GameObject SABar;
    [SerializeField] private Image frontSABar;
    [SerializeField] private TextMeshProUGUI SAText;

    [Header("Damage Overlay")]
    [SerializeField] private Image damageOverlay;
    [SerializeField] private float duration;
    [SerializeField] private float fadeSpeed;

    [Header("Main Skill")]
    [SerializeField] private Image skillIcon;

    [Header("Sub Skill")]
    [SerializeField] private GameObject subSkillIconBG;
    [SerializeField] private Image subSkillCoolDown;
    [SerializeField] private TextMeshProUGUI subSkillText;

    [Header("Dash")]
    [SerializeField] private GameObject dashIconBG;
    [SerializeField] private Image dashCoolDown;

    [Header("Weapon")]
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI[] weaponNum = new TextMeshProUGUI[3];

    [Header("Ammo")]
    [SerializeField] private GameObject normalAmmo;
    [SerializeField] private Image normalAmmoFront;
    [SerializeField] private GameObject largeAmmo;
    [SerializeField] private Image largeAmmoFront;
    [SerializeField] private GameObject specialAmmo;
    [SerializeField] private Image specialAmmoFront;
    [SerializeField] private TextMeshProUGUI ammoText;

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        
    }

    private void Start()
    {
        damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, 0);
        //InitSABar();
    }

    private void Update()
    {
        
    }

    public void SetPromptText(string promptMessage)
    {
        promptText.text = promptMessage;
    }

    #region HPBar
    public void UpdateHPType(EnumTypes.HPType hpType)
    {
        Color color;
        if (hpType == EnumTypes.HPType.Shield)
        {
            SABar.SetActive(true);
            ColorUtility.TryParseHtmlString("#00FFFF", out color);
            frontSABar.color = color;
            SAText.color = color;
        }
        else if (hpType == EnumTypes.HPType.Armor)
        {
            SABar.SetActive(true);
            ColorUtility.TryParseHtmlString("#FFB000", out color);
            frontSABar.color = color;
            SAText.color = color;
        }
        else
        {
            SABar.SetActive(false);
        }
    }

    public void ShowDamageOverlay(float HP)
    {
        damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, 1f);
        if (HP < 30f) return;
        damageOverlay.DOColor(new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, 0f), 1f);
    }

    public void UpdateHPBar(int HP, int maxHP)
    {
        HPText.text = string.Concat(HP, " / ", maxHP);
        float fillFront = frontHPBar.fillAmount;
        float fillBack = backHPBar.fillAmount;
        float HPFraction = (float)HP / maxHP;
        if (fillBack > HPFraction)
        {
            frontHPBar.fillAmount = HPFraction;
            backHPBar.color = Color.red;
            backHPBar.DOFillAmount(HPFraction, 0.5f);
        }
        else if (fillFront < HPFraction)
        {
            backHPBar.fillAmount = HPFraction;
            backHPBar.color = Color.green;
            frontHPBar.DOFillAmount(HPFraction, 0.5f);
        }
    }

    public void UpdateSABar(int SA, int maxSA)
    {
        float SAFraction = (float)SA / maxSA;
        SAText.text = string.Concat(SA, " / ", maxSA);
        frontSABar.DOFillAmount(SAFraction, 0.5f);
    }
    #endregion

    #region Skill
    public void UpdateMainSkillCoolDown(float skillTimer, float skillCoolTime)
    {
        skillIcon.fillAmount = skillTimer / skillCoolTime;
        skillIcon.DOFillAmount(1f, skillCoolTime - skillTimer);
        //if (_skillTimerCoroutine != null)
        //{
        //    StopCoroutine(_skillTimerCoroutine);
        //}
        //_skillTimerCoroutine = StartCoroutine(UpdateMainSkillTimer(skillTimer, skillCoolTime));
    }

    public void UpdateSubSkillText(int num)
    {
        subSkillText.text = num.ToString();
    }

    public void UpdateSubSkillCoolDown(float skillTimer, float skillCoolTime)
    {
        subSkillIconBG.SetActive(true);
        subSkillCoolDown.fillAmount = 1f - skillTimer / skillCoolTime;
        subSkillCoolDown.DOFillAmount(0f, skillCoolTime - skillTimer).OnComplete(() => subSkillIconBG.SetActive(false));
        //if (_subSkillTimerCoroutine != null)
        //{
        //    StopCoroutine(_subSkillTimerCoroutine);
        //}
        //_subSkillTimerCoroutine = StartCoroutine(UpdateSubSkillTimer(skillTimer, skillCoolTime));
    }

    public void UpdateDashCoolDown(float dashTimer, float dashCoolTime)
    {
        dashIconBG.SetActive(true);
        dashCoolDown.fillAmount = 1f - dashTimer / dashCoolTime;
        dashCoolDown.DOFillAmount(0f, dashCoolTime - dashTimer).OnComplete(() => dashIconBG.SetActive(false));
        //if (_dashTimerCoroutine != null)
        //{
        //    StopCoroutine(_dashTimerCoroutine);
        //}
        //_dashTimerCoroutine = StartCoroutine(UpdateDashTimer(dashTimer, dashCoolTime));
    }
    #endregion

    #region Weapon
    public void UpdateCurrentAmmoType(EnumTypes.AmmoType ammoType)
    {
        Color color;
        normalAmmo.transform.localScale = Vector3.one;
        largeAmmo.transform.localScale = Vector3.one;
        specialAmmo.transform.localScale = Vector3.one;
        switch (ammoType)
        {
            case EnumTypes.AmmoType.Normal:
                normalAmmo.transform.localScale = Vector3.one * 1.2f;
                // #8ED973
                //normalAmmo.transform.localScale = normalAmmo.transform.localScale * 1.2f;
                ColorUtility.TryParseHtmlString("#8ED973", out color);
                break;
            case EnumTypes.AmmoType.Large:
                largeAmmo.transform.localScale = Vector3.one * 1.2f;
                // #61CBF4
                //largeAmmo.transform.localScale = normalAmmo.transform.localScale * 1.2f;
                ColorUtility.TryParseHtmlString("#61CBF4", out color);
                break;
            case EnumTypes.AmmoType.Special:
                specialAmmo.transform.localScale = Vector3.one * 1.2f;
                // #FFFF00
                //specialAmmo.transform.localScale = normalAmmo.transform.localScale * 1.2f;
                ColorUtility.TryParseHtmlString("#FFFF00", out color);
                break;
            default:
                // #FFFFFF
                ColorUtility.TryParseHtmlString("#FFFFFF", out color);
                break;
        }
        ammoText.color = color;
    }

    public void UpdateAmmoText(EnumTypes.AmmoType ammoType, int ammo, int maxAmmo)
    {
        switch (ammoType)
        {
            case EnumTypes.AmmoType.Normal:
                ammoText.text = string.Concat(ammo, "/", maxAmmo);
                break;
            case EnumTypes.AmmoType.Large:
                ammoText.text = string.Concat(ammo, "/", maxAmmo);
                break;
            case EnumTypes.AmmoType.Special:
                ammoText.text = string.Concat(ammo, "/", maxAmmo);
                break;
            default:
                ammoText.text = string.Concat(ammo, "/¡Ä");
                break;
        }
    }

    public void UpdateAmmoFillAmount(EnumTypes.AmmoType ammoType, int ammo, int maxAmmo)
    {
        switch (ammoType)
        {
            case EnumTypes.AmmoType.Normal:
                normalAmmoFront.fillAmount = (float)ammo / maxAmmo;
                break;
            case EnumTypes.AmmoType.Large:
                largeAmmoFront.fillAmount = (float)ammo / maxAmmo;
                break;
            case EnumTypes.AmmoType.Special:
                specialAmmoFront.fillAmount = (float)ammo / maxAmmo;
                break;
        }
    }

    public void UpdateWeaponNum(int num)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#808080", out color);
        for (int i = 0; i < 3; i++)
            weaponNum[i].color = color;
        weaponNum[num].color = Color.white;
    }

    public void UpdateWeaponIcon(Sprite icon)
    {
        weaponIcon.sprite = icon;
    }
    #endregion
}
