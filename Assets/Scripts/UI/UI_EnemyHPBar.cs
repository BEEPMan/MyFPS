using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UI_EnemyHPBar : MonoBehaviour
{
    [Header("HP Bar")]
    [SerializeField] private Image frontHPBar;
    [SerializeField] private Transform buffList;

    [Header("Shield/Armor Bar")]
    [SerializeField] private GameObject SABar;
    [SerializeField] private Image frontSABar;

    [SerializeField] private GameObject miasmaStack;
    [SerializeField] private TextMeshProUGUI miasmaText;

    void Start()
    {
        InitBuffIcons();
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }

    public void InitHPBer()
    {
        frontHPBar.fillAmount = 1f;
        Transform[] damageTexts = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            damageTexts[i] = transform.GetChild(i);
        }
        foreach(Transform child in damageTexts)
        {
            if(child.name == "DamageText")
            {
                ObjectPool.Instance.Push(child.gameObject);
            }
        }
        ClearBuffIcon();
    }

    public void UpdateHPType(EnumTypes.HPType hpType)
    {
        Color color;
        if (hpType == EnumTypes.HPType.Shield)
        {
            SABar.SetActive(true);
            ColorUtility.TryParseHtmlString("#00FFFF", out color);
            frontSABar.color = color;
        }
        else if (hpType == EnumTypes.HPType.Armor)
        {
            SABar.SetActive(true);
            ColorUtility.TryParseHtmlString("#FFB000", out color);
            frontSABar.color = color;
        }
        else
        {
            SABar.SetActive(false);
        }
    }

    public void InitBuffIcons()
    {
        foreach (Image icon in buffList.GetComponentsInChildren<Image>())
        {
            if (icon.name == "BuffList" || icon.name == "Background") continue;
            icon.gameObject.SetActive(false);
        }
    }

    public void UpdateHPBar(int HP, int maxHP)
    {
        float HPFraction = (float)HP / maxHP;
        frontHPBar.DOFillAmount(HPFraction, 0.5f);
    }

    public void UpdateSABar(int SA, int maxSA)
    {
        float SAFraction = (float)SA / maxSA;
        frontSABar.DOFillAmount(SAFraction, 0.5f);
    }

    public void PopDamageText(int damage)
    {
        GameObject go = ObjectPool.Instance.Pop("DamageText", Vector3.zero, Quaternion.identity, transform);
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        Vector3 randomPos = new Vector3(Random.Range(-90f, 90f), Random.Range(45f, 90f), 0f);
        rectTransform.anchoredPosition3D = randomPos;
        rectTransform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        rectTransform.localScale = Vector3.one;
        go.GetComponent<UI_FloatingDamage>().SetDamageText(damage);
    }

    public void EnableBuffIcon(string buffName)
    {
        Transform buffIcon;
        if (buffIcon = buffList.Find(buffName))
            buffIcon.gameObject.SetActive(true);
    }

    public void DisableBuffIcon(string buffName)
    {
        Transform buffIcon;
        if (buffIcon = buffList.Find(buffName))
            buffIcon.gameObject.SetActive(false);
    }

    public void ClearBuffIcon()
    {
        for (int i = 0; i < buffList.childCount; i++)
        {
            buffList.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void EnableMiasmaStack()
    {
        miasmaStack.SetActive(true);
    }

    public void UpdateMiasmaCount(int miasmaCount)
    {
        miasmaText.text = miasmaCount.ToString();
    }

    public void DisableMiasmaStack()
    {
        miasmaStack.SetActive(false);
    }
}
