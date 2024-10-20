using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Inventory_Item : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Scroll scroll;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.ScrollInventory.SetScrollDetail(scroll);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.ScrollInventory.ClearScrollDetail();
    }
}
