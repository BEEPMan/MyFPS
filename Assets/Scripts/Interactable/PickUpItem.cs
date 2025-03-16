using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : Interactable
{
    public Item Item;

    public void Awake()
    {
        promptMessage = Item.ItemName;
    }

    protected override void Interact(PlayerController player)
    {
        Item.Gain(player);
        Destroy(gameObject);
    }
}
