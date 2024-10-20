using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_FloatingDamage : MonoBehaviour
{
    public TextMeshProUGUI damageText;

    void OnEnable()
    {
        StartCoroutine(FadeOut());
    }

    void Update()
    {

    }

    public void SetDamageText(int damage)
    {
        damageText.text = damage.ToString();
    }

    IEnumerator FadeOut()
    {
        float timer = 0f;
        Color color = damageText.color;
        color.a = 1f;
        damageText.color = color;
        RectTransform rectTransform = GetComponent<RectTransform>();
        while (timer < 1f)
        {
            color.a = 1f - timer;
            damageText.color = color;
            rectTransform.anchoredPosition += Vector2.up * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        color.a = 0f;
        damageText.color = color;
        ObjectPool.Instance.Push(gameObject);
    }
}
