using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Inventory_Item : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI_Inventory_Scroll _scrollInventory;

    public string ScrollName;

    public void Init(string scrollName, UI_Inventory_Scroll parent)
    {
        _scrollInventory = parent;
        ScrollName = scrollName;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _scrollInventory.SetScrollDetail(ScrollName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _scrollInventory.ClearScrollDetail();
    }
}
