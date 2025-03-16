using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peddler : Interactable
{
    public Item[] SellItems;

    private bool isFirstInteract;

    void Start()
    {
        //InitAfterLoad().Forget();
        SellItems = new Item[Global.NumOfPeddlerItems];
        SellItems[0] = ItemManager.Instance.FindPeddlerItem("Health Kit");
        SellItems[1] = ItemManager.Instance.FindPeddlerItem("Ammo Supply");
        //for (int i = 2; i < 2 + (Global.NumOfPeddlerItems - 2) / 2; i++)
        //{
        //    SellItems[i] = ItemManager.Instance.GetRandomItemName(EnumTypes.ItemType.Scroll);
        //}
        for (int i = 2; i < Global.NumOfPeddlerItems; i++)
        {
            SellItems[i] = ItemManager.Instance.GetRandomWeapon();
        }
        isFirstInteract = true;
    }

    void Update()
    {
        
    }

    protected override void Interact(PlayerController player)
    {
        UIManager.Instance.ShowPanel("UI_Peddler");
        if (isFirstInteract)
        {
            UI_Peddler peddlerUI = UIManager.Instance.PopUpsInScene["UI_Peddler"] as UI_Peddler;
            for (int i = 0; i < Global.NumOfPeddlerItems; i++)
            {
                peddlerUI.AddItemSlot(SellItems[i]);
            }
            isFirstInteract = false;
        }
    }
}
