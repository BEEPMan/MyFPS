using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Peddler_Item : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI_Peddler _peddler;

    public Item Item { get; private set; }

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI priceText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _peddler.SetItemDetail(Item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _peddler.ClearItemDetail();
    }

    public void Init(Item item, UI_Peddler peddler)
    {
        _peddler = peddler;
        Item = item;
        Color color = icon.color;
        color.a = 1f;
        icon.color = color;
        icon.sprite = Item.Icon;
        priceText.text = Item.Price.ToString();
        if (Item.Price > GameManager.Instance.Player.Coin.Value)
            ChangeTextColor(Color.red);
        else
            ChangeTextColor(Color.white);

    }

    public void ChangeTextColor(Color color)
    {
        priceText.color = color;
    }

    public void BuyItem()
    {
        if (GameManager.Instance.Player.Coin.Value < Item.Price) return;
        GameManager.Instance.Player.GainCoin(-Item.Price);
        
        Item.Gain(GameManager.Instance.Player);
        _peddler.OnItemSell();
        Destroy(gameObject);
    }
}
