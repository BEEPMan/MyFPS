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
    [SerializeField] private GameObject scrolls;
    private Image[] scrollIcons;

    [Header("Scroll Details")]
    [SerializeField] private Image scrollDetailIcon;
    [SerializeField] private TextMeshProUGUI scrollDetailName;
    [SerializeField] private TextMeshProUGUI scrollDetailDescription;

    protected override void Init()
    {
        scrollIcons = new Image[scrolls.transform.childCount];
    }

    private void Update()
    {
        
    }

    public override void OnPopUp()
    {
        UpdateScrollInventory();
    }

    public void UpdateScrollInventory()
    {
        int i = 0;
        foreach(Scroll scroll in GameManager.Instance.Player.ScrollManager.Scrolls.Values)
        {
            Color color = scrollIcons[i].color;
            color.a = 1f;
            scrollIcons[i].color = color;
            Sprite icon = scroll.Icon;
            scrollIcons[i].sprite = icon;
            if (scrollIcons[i].GetComponent<UI_Inventory_Item>() != null)
            {
                Destroy(scrollIcons[i].GetComponent<UI_Inventory_Item>());
            }
            scrollIcons[i].AddComponent<UI_Inventory_Item>().Init(scroll.ItemName, this);
            i++;
        }
    }

    public void SetScrollDetail(string scrollName)
    {
        Scroll scroll = ItemManager.Instance.FindScroll(scrollName);

        Color color = scrollDetailIcon.color;
        color.a = 1f;
        scrollDetailIcon.color = color;
        scrollDetailIcon.sprite = scroll.Icon;
        scrollDetailName.text = scroll.ItemName;
        scrollDetailDescription.text = scroll.Description;
    }

    public void ClearScrollDetail()
    {
        Color color = scrollDetailIcon.color;
        color.a = 0f;
        scrollDetailIcon.color = color;
        scrollDetailName.text = "";
        scrollDetailDescription.text = "";
    }

    public void ChangeTab()
    {
        UIManager.Instance.HidePanel("UI_Inventory_Scroll");
        UIManager.Instance.ShowPanel("UI_Inventory_Weapon");
    }
}
