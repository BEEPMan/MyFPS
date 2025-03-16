using EnumTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    public ItemGroup ItemGroups;
    private Dictionary<string, Weapon> WeaponList;
    private Dictionary<string, Scroll> ScrollList;
    private Dictionary<string, Item> PickupItemList;
    private Dictionary<string, Item> PeddlerItemList;

    protected override void Awake()
    {
        base.Awake();
        WeaponList = new Dictionary<string, Weapon>();
        ScrollList = new Dictionary<string, Scroll>();
        PickupItemList = new Dictionary<string, Item>();
        PeddlerItemList = new Dictionary<string, Item>();
        foreach (Weapon weapon in ItemGroups.Weapons)
        {
            WeaponList.Add(weapon.ItemName, weapon);
        }
        foreach (Scroll scroll in ItemGroups.Scrolls)
        {
            ScrollList.Add(scroll.ItemName, scroll);
        }
        foreach (Item item in ItemGroups.PickupItems)
        {
            PickupItemList.Add(item.ItemName, item);
        }
        foreach (Item item in ItemGroups.PeddlerItems)
        {
            PeddlerItemList.Add(item.ItemName, item);
        }
    }

    public Weapon FindWeapon(string name)
    {
        Weapon weapon;
        if (!WeaponList.TryGetValue(name, out weapon))
            return null;
        return weapon;
    }

    public Scroll FindScroll(string name)
    {
        Scroll scroll;
        if (!ScrollList.TryGetValue(name, out scroll))
            return null;
        return scroll;
    }

    public Item FindPickupItem(string name)
    {
        Item item;
        if (!PickupItemList.TryGetValue(name, out item))
            return null;
        return item;
    }

    public Item FindPeddlerItem(string name)
    {
        Item item;
        if (!PeddlerItemList.TryGetValue(name, out item))
            return null;
        return item;
    }

    public Weapon GetRandomWeapon()
    {
        return ItemGroups.Weapons[UnityEngine.Random.Range(0, ItemGroups.Weapons.Count)];
    }

    public Scroll GetRandomScroll()
    {
        return ItemGroups.Scrolls[UnityEngine.Random.Range(0, ItemGroups.Scrolls.Count)];
    }

    public Item GetRandomPickupItem()
    {
        return ItemGroups.PickupItems[UnityEngine.Random.Range(0, ItemGroups.PickupItems.Count)];
    }

    public Item GetRandomPeddlerItem()
    {
        return ItemGroups.PeddlerItems[UnityEngine.Random.Range(0, ItemGroups.PeddlerItems.Count)];
    }

    public void MakePickupItem(Item item, Vector3 position)
    {
        GameObject pickup = ObjectPool.Instance.Pop($"PickupItem/{item.ItemName}", position, Quaternion.identity);
        pickup.GetComponent<Rigidbody>().AddForce(Vector3.up * 3.0f, ForceMode.Impulse);

        Rigidbody rb = pickup.GetComponent<Rigidbody>();
        Vector3 dir = UnityEngine.Random.insideUnitSphere;
        dir.y = MathF.Abs(dir.y);

        rb.AddForce(dir * 3f, ForceMode.Impulse);
        rb.AddForce(dir * 3f, ForceMode.Impulse);

        float rand = UnityEngine.Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(rand, rand, rand) * 10f);
    }
}
