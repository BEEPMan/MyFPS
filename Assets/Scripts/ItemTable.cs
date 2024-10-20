using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTable : MonoBehaviour
{
    private static ItemTable instance = null;
    public static ItemTable Instance
    {
        get
        {
            if (instance == null) return null;
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public Item[] weapons;
    public Item[] scrolls;
    public Item[] ammos;
    public Item healthKit;

    public Item GetRandomItem(ItemType type)
    {
        switch (type)
        {
            case ItemType.Weapon:
                return weapons[Random.Range(0, weapons.Length)];
            case ItemType.Scroll:
                return scrolls[Random.Range(0, scrolls.Length)];
            case ItemType.AmmoSupply:
                return ammos[Random.Range(1, ammos.Length)];
            case ItemType.HealthKit:
                return healthKit;
            default:
                return null;
        }
    }

    public Item FindItem(string name)
    {
        foreach (Item item in weapons)
        {
            if (item == null) continue;
            if (item.itemName == name) return item;
        }
        foreach (Item item in scrolls)
        {
            if (item == null) continue;
            if (item.itemName == name) return item;
        }
        foreach (Item item in ammos)
        {
            if (item == null) continue;
            if (item.itemName == name) return item;
        }
        if (healthKit.itemName == name) return healthKit;
        else return null;
    }
}
