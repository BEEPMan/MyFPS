using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Peddler_Item : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;

    public Image icon;
    public TextMeshProUGUI priceText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.Peddler.SetItemDetail(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.Peddler.ClearItemDetail();
    }

    public void SetSlot()
    {
        Color color = icon.color;
        color.a = 1f;
        icon.color = color;
        icon.sprite = item.icon;
        priceText.text = item.price.ToString();
    }

    public void BuyItem()
    {
        if (Player.Instance.coin < item.price) return;
        Player.Instance.coin -= item.price;
        Player.Instance.GetItem(item);
        UIManager.Instance.Peddler.ClearItemDetail();
        UIManager.Instance.Peddler.SetCoinText(Player.Instance.coin);
        Destroy(gameObject);
    }
}
