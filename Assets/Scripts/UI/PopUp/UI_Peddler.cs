using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Peddler : UI_PopUp
{
    public TextMeshProUGUI coinText;

    [Header("Item Slots")]
    [SerializeField] private GameObject itemSlot;
    [SerializeField] private Transform contents;

    [Header("Item Details")]
    [SerializeField] private Image itemDetailIcon;
    [SerializeField] private TextMeshProUGUI itemDetailName;
    [SerializeField] private TextMeshProUGUI itemDetailDescription;

    protected override void Init()
    {
    }

    public override void OnPopUp()
    {
        UpdateCoinText(GameManager.Instance.Player.Coin.Value);
    }

    public void AddItemSlot(Item item)
    {
        GameObject go = Instantiate(itemSlot);
        UI_Peddler_Item uiItem = go.GetComponent<UI_Peddler_Item>();
        uiItem.Init(item, this);
        go.transform.SetParent(contents, false);
    }

    public void SetItemDetail(Item item)
    {
        Color color = itemDetailIcon.color;
        color.a = 1f;
        itemDetailIcon.color = color;
        itemDetailIcon.sprite = item.Icon;
        itemDetailName.text = item.ItemName;
        itemDetailDescription.text = item.Description;
    }

    public void UpdateCoinText(int coin)
    {
        coinText.text = coin.ToString();
    }

    public void OnItemSell()
    {
        ClearItemDetail();
        UpdateCoinText(GameManager.Instance.Player.Coin.Value);
        foreach(Transform slot in contents)
        {
            UI_Peddler_Item data = slot.GetComponent<UI_Peddler_Item>();
            if (data != null)
            {
                if (data.Item.Price > GameManager.Instance.Player.Coin.Value)
                    data.ChangeTextColor(Color.red);
                else
                    data.ChangeTextColor(Color.white);
            }
        }
    }

    public void ClearItemDetail()
    {
        Color color = itemDetailIcon.color;
        color.a = 0f;
        itemDetailIcon.color = color;
        itemDetailName.text = "";
        itemDetailDescription.text = "";
    }
}
