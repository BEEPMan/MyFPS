using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("Prompt Text")]
    public TextMeshProUGUI promptText;

    [Header("HP Bar")]
    public float chipSpeed = 2f;
    public Image frontHPBar;
    public Image backHPBar;
    public TextMeshProUGUI HPText;

    [Header("Shield/Armor Bar")]
    public GameObject SABar;
    public Image frontSABar;
    public TextMeshProUGUI SAText;

    [Header("Damage Overlay")]
    public Image damageOverlay;
    public float duration;
    public float fadeSpeed;

    [Header("Main Skill")]
    public Image skillIcon;

    [Header("Sub Skill")]
    public GameObject subSkillIconBG;
    public Image subSkillCoolDown;
    public TextMeshProUGUI subSkillText;

    [Header("Dash")]
    public GameObject dashIconBG;
    public Image dashCoolDown;

    [Header("Weapon")]
    public Image weaponIcon;
    public TextMeshProUGUI[] weaponNum = new TextMeshProUGUI[3];

    [Header("Ammo")]
    public GameObject normalAmmo;
    public Image normalAmmoFront;
    public GameObject largeAmmo;
    public Image largeAmmoFront;
    public GameObject specialAmmo;
    public Image specialAmmoFront;
    public TextMeshProUGUI ammoText;

    private float lerpTimer;
    private float durationTimer;

    private Coroutine _HPBarCoroutine = null;
    private Coroutine _damageOverlayCoroutine = null;
    private Coroutine _skillTimerCoroutine = null;
    private Coroutine _subSkillTimerCoroutine = null;
    private Coroutine _dashTimerCoroutine = null;

    private void Start()
    {
        damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, 0);
        InitSABar();
    }

    private void Update()
    {
        
    }

    public void InitSABar()
    {
        Color color;
        if (Player.Instance.PStat.healthType == SecondaryType.Shield)
        {
            SABar.SetActive(true);
            ColorUtility.TryParseHtmlString("#00FFFF", out color);
            frontSABar.color = color;
            SAText.color = color;
            //SAText.text = string.Concat(Player.Instance.PStat.SA, " / ", Player.Instance.PStat.MaxSA);
        }
        else if (Player.Instance.PStat.healthType == SecondaryType.Armor)
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

    public void SetDamageOverlay(float HP)
    {
        durationTimer = 0f;
        damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, 1);
        if (_damageOverlayCoroutine != null)
        {
            StopCoroutine(_damageOverlayCoroutine);
        }
        _damageOverlayCoroutine = StartCoroutine(UpdateDamageOverlay(HP));
    }

    public void SetPromptText(string promptMessage)
    {
        promptText.text = promptMessage;
    }

    public void SetHPBar(float HP, float maxHP)
    {
        HPText.text = string.Concat(HP, " / ", maxHP);
        if (_HPBarCoroutine != null)
        {
            StopCoroutine(_HPBarCoroutine);
        }
        lerpTimer = 0f;
        _HPBarCoroutine = StartCoroutine(UpdateHPUI(HP, maxHP));
    }

    public void SetSABar(float SA, float maxSA)
    {
        float HPFraction = SA / maxSA;
        SAText.text = string.Concat(SA, " / ", maxSA);
        frontSABar.fillAmount = HPFraction;
    }

    public void SetMainSkillIcon(float skillTimer, float skillCoolTime)
    {
        if (_skillTimerCoroutine != null)
        {
            StopCoroutine(_skillTimerCoroutine);
        }
        _skillTimerCoroutine = StartCoroutine(UpdateMainSkillTimer(skillTimer, skillCoolTime));
    }

    public void SetSubSkillIcon(float skillTimer, float skillCoolTime)
    {
        if (_subSkillTimerCoroutine != null)
        {
            StopCoroutine(_subSkillTimerCoroutine);
        }
        _subSkillTimerCoroutine = StartCoroutine(UpdateSubSkillTimer(skillTimer, skillCoolTime));
    }

    public void SetDashIcon(float dashTimer, float dashCoolTime)
    {
        if (_dashTimerCoroutine != null)
        {
            StopCoroutine(_dashTimerCoroutine);
        }
        _dashTimerCoroutine = StartCoroutine(UpdateDashTimer(dashTimer, dashCoolTime));
    }

    public void SetAmmoText(AmmoType ammoType, int ammo, int maxAmmo)
    {
        Color color;
        switch (ammoType)
        {
            case AmmoType.Infinite:
                // #FFFFFF
                ColorUtility.TryParseHtmlString("#FFFFFF", out color);
                ammoText.color = color;
                ammoText.text = string.Concat(ammo, "/¡Ä");
                break;
            case AmmoType.Normal:
                // #8ED973
                //normalAmmo.transform.localScale = normalAmmo.transform.localScale * 1.2f;
                ColorUtility.TryParseHtmlString("#8ED973", out color);
                ammoText.color = color;
                ammoText.text = string.Concat(ammo, "/", maxAmmo);
                break;
            case AmmoType.Large:
                // #61CBF4
                //largeAmmo.transform.localScale = normalAmmo.transform.localScale * 1.2f;
                ColorUtility.TryParseHtmlString("#61CBF4", out color);
                ammoText.color = color;
                ammoText.text = string.Concat(ammo, "/", maxAmmo);
                break;
            case AmmoType.Special:
                // #FFFF00
                //specialAmmo.transform.localScale = normalAmmo.transform.localScale * 1.2f;
                ColorUtility.TryParseHtmlString("#FFFF00", out color);
                ammoText.color = color;
                ammoText.text = string.Concat(ammo, "/", maxAmmo);
                break;
        }
    }

    public void SetAmmoFillAmount(AmmoType ammoType, int ammo, int maxAmmo)
    {
        switch (ammoType)
        {
            case AmmoType.Normal:
                normalAmmoFront.fillAmount = (float)ammo / maxAmmo;
                break;
            case AmmoType.Large:
                largeAmmoFront.fillAmount = (float)ammo / maxAmmo;
                break;
            case AmmoType.Special:
                specialAmmoFront.fillAmount = (float)ammo / maxAmmo;
                break;
        }
    }

    public void SetAmmoSize(AmmoType ammoType)
    {
        normalAmmo.transform.localScale = Vector3.one;
        largeAmmo.transform.localScale = Vector3.one;
        specialAmmo.transform.localScale = Vector3.one;
        switch (ammoType)
        {
            case AmmoType.Normal:
                normalAmmo.transform.localScale = Vector3.one * 1.2f;
                break;
            case AmmoType.Large:
                largeAmmo.transform.localScale = Vector3.one * 1.2f;
                break;
            case AmmoType.Special:
                specialAmmo.transform.localScale = Vector3.one * 1.2f;
                break;
        }
    }

    public void SetWeaponNum(int num)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#808080", out color);
        for (int i = 0; i < 3; i++)
            weaponNum[i].color = color;
        weaponNum[num].color = Color.white;
    }

    public void SetWeaponIcon(Sprite icon)
    {
        weaponIcon.sprite = icon;
    }

    public void SetSubSkillText(int num)
    {
        subSkillText.text = num.ToString();
    }

    private IEnumerator UpdateHPUI(float HP, float maxHP)
    {
        float fillFront = frontHPBar.fillAmount;
        float fillBack = backHPBar.fillAmount;
        float HPFraction = HP / maxHP;
        while (fillBack > HPFraction)
        {
            fillBack = backHPBar.fillAmount;
            frontHPBar.fillAmount = HPFraction;
            backHPBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            backHPBar.fillAmount = Mathf.Lerp(fillBack, HPFraction, percentComplete);
            yield return null;
        }
        while (fillFront < HPFraction)
        {
            fillFront = frontHPBar.fillAmount;
            backHPBar.fillAmount = HPFraction;
            backHPBar.color = Color.green;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            frontHPBar.fillAmount = Mathf.Lerp(fillFront, HPFraction, percentComplete);
            yield return null;
        }
        _HPBarCoroutine = null;
    }

    private IEnumerator UpdateDamageOverlay(float HP)
    {
        while (damageOverlay.color.a > 0)
        {
            if (HP < 30f)
                yield break;
            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                float tempAlpha = damageOverlay.color.a;
                tempAlpha -= Time.deltaTime * fadeSpeed;
                damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, tempAlpha);
            }
            yield return null;
        }
        _damageOverlayCoroutine = null;
    }

    private IEnumerator UpdateMainSkillTimer(float skillTimer, float skillCoolTime)
    {
        float timer = skillTimer;
        while (timer < skillCoolTime)
        {
            skillIcon.fillAmount = timer / skillCoolTime;
            timer += Time.deltaTime;
            yield return null;
        }
        skillIcon.fillAmount = 1f;
        _skillTimerCoroutine = null;
    }

    private IEnumerator UpdateSubSkillTimer(float skillTimer, float skillCoolTime)
    {
        subSkillIconBG.SetActive(true);
        float timer = skillTimer;
        while (timer < skillCoolTime)
        {
            subSkillCoolDown.fillAmount = timer / skillCoolTime;
            timer += Time.deltaTime;
            yield return null;
        }
        subSkillCoolDown.fillAmount = 0f;
        subSkillIconBG.SetActive(false);
        _subSkillTimerCoroutine = null;
    }

    private IEnumerator UpdateDashTimer(float skillTimer, float skillCoolTime)
    {
        dashIconBG.SetActive(true);
        float timer = skillTimer;
        while (timer < skillCoolTime)
        {
            dashCoolDown.fillAmount = timer / skillCoolTime;
            timer += Time.deltaTime;
            yield return null;
        }
        dashCoolDown.fillAmount = 0f;
        dashIconBG.SetActive(false);
        _dashTimerCoroutine = null;
    }
}
