using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Peddler : UI_PopUp
{
    public TextMeshProUGUI coinText;

    [Header("Item Slots")]
    public GameObject itemSlot;
    public Transform contents;

    [Header("Item Details")]
    public Image itemDetailIcon;
    public TextMeshProUGUI itemDetailName;
    public TextMeshProUGUI itemDetailDescription;

    [SerializeField]
    private int _numOfItems = 8;

    private bool isOpenFirstTime = true;

    void Update()
    {
        
    }

    public override void OnPopUp()
    {
        if (isOpenFirstTime)
        {
            InitItemList();
            isOpenFirstTime = false;
        }
    }

    public void InitItemList()
    {
        for (int i = 0; i < contents.childCount; i++)
        {
            Destroy(contents.GetChild(0).gameObject);
        }
        AddItemSlot(ItemTable.Instance.healthKit);
        AddItemSlot(ItemTable.Instance.GetRandomItem(ItemType.AmmoSupply));
        for (int i = 2; i < _numOfItems; i++)
        {
            int pick = Random.Range(0, 2);
            if (pick == 0)
            {
                AddItemSlot(ItemTable.Instance.GetRandomItem(ItemType.Weapon));
            }
            else
            {
                AddItemSlot(ItemTable.Instance.GetRandomItem(ItemType.Scroll));
            }
        }
        coinText.text = Player.Instance.coin.ToString();
    }

    public void AddItemSlot(Item item)
    {
        GameObject go = Instantiate(itemSlot);
        UI_Peddler_Item uiItem = itemSlot.GetComponent<UI_Peddler_Item>();
        uiItem.item = item;
        uiItem.SetSlot();
        go.transform.SetParent(contents, false);
    }

    public void SetItemDetail(Item item)
    {
        Color color = itemDetailIcon.color;
        color.a = 1f;
        itemDetailIcon.color = color;
        itemDetailIcon.sprite = item.icon;
        itemDetailName.text = item.name;
        itemDetailDescription.text = item.description;
    }

    public void SetCoinText(int coin)
    {
        coinText.text = coin.ToString();
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
