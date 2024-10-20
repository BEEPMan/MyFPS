using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UI_EnemyHPBar : MonoBehaviour
{
    [Header("HP Bar")]
    public float chipSpeed = 2f;
    public Image frontHPBar;
    public Image backHPBar;
    public Transform buffList;

    public GameObject miasmaStack;
    public TextMeshProUGUI miasmaText;

    private float lerpTimer;

    private Enemy _enemy;

    void Start()
    {
        InitBuffIcons();
    }

    void Update()
    {
        if (_enemy != null)
            UpdateHPUI();
        transform.LookAt(Player.Instance.mainCamera.transform.position);
    }

    public void InitHPBer()
    {
        frontHPBar.fillAmount = 1f;
        backHPBar.fillAmount = 1f;
        Transform[] fucker = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            fucker[i] = transform.GetChild(i);
        }
        foreach(Transform child in fucker)
        {
            if(child.name == "DamageText")
            {
                ObjectPool.Instance.Push(child.gameObject);
            }
        }
        ClearBuffIcon();
    }

    public void InitBuffIcons()
    {
        foreach (Image icon in buffList.GetComponentsInChildren<Image>())
        {
            if (icon.name == "BuffList" || icon.name == "Background") continue;
            icon.gameObject.SetActive(false);
        }
    }

    public void SetLerpTimer()
    {
        lerpTimer = 0f;
    }

    public void UpdateHPUI()
    {
        float fillFront = frontHPBar.fillAmount;
        float fillBack = backHPBar.fillAmount;
        float HPFraction = _enemy.Stat.HP / _enemy.Stat.MaxHP;
        if (fillBack > HPFraction)
        {
            frontHPBar.fillAmount = HPFraction;
            backHPBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            backHPBar.fillAmount = Mathf.Lerp(fillBack, HPFraction, percentComplete);
        }
        if (fillFront < HPFraction)
        {
            backHPBar.fillAmount = HPFraction;
            backHPBar.color = Color.green;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            frontHPBar.fillAmount = Mathf.Lerp(fillFront, HPFraction, percentComplete);
        }
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

    public void SetEnemy(Enemy enemy)
    {
        _enemy = enemy;
    }
}
