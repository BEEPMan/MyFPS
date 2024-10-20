using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Interactable
{
    public Item item;

    void Start()
    {
        if (item != null)
        {
            promptMessage = item.itemName;
        }
    }

    void Update()
    {
        
    }

    protected override void Interact()
    {
        if (item.type == ItemType.Weapon)
            Player.Instance.GetItem(transform.GetChild(0).gameObject);
        else
            Player.Instance.GetItem(item);
        Destroy(gameObject);
    }

    public void MakePickup(GameObject go)
    {
        item = ItemTable.Instance.FindItem(go.name);
        promptMessage = item.itemName;
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.layer = 0;
    }

    public void MakePickup(Item dropItem)
    {
        item = dropItem;
        promptMessage = item.itemName;
        GameObject go;
        switch (item.type)
        {
            case ItemType.Weapon:
                go = Instantiate(Resources.Load("Prefabs/Weapons/" + item.itemName) as GameObject);
                int index = go.name.IndexOf("(Clone)");
                if (index > 0)
                    go.name = go.name.Substring(0, index);
                go.transform.SetParent(transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.layer = 0;
                break;
            case ItemType.Scroll:
                go = ObjectPool.Instance.Pop("Scroll", Vector3.zero, Quaternion.identity, transform);
                break;
            case ItemType.AmmoSupply:
                switch(item.ammoType)
                {
                    case AmmoType.Normal:
                        go = ObjectPool.Instance.Pop("NormalAmmo", Vector3.zero, Quaternion.identity, transform);
                        break;
                    case AmmoType.Large:
                        go = ObjectPool.Instance.Pop("LargeAmmo", Vector3.zero, Quaternion.identity, transform);
                        break;
                    case AmmoType.Special:
                        go = ObjectPool.Instance.Pop("SpecialAmmo", Vector3.zero, Quaternion.identity, transform);
                        break;
                }
                break;
            case ItemType.HealthKit:
                go = ObjectPool.Instance.Pop("HealthKit", Vector3.zero, Quaternion.identity, transform);
                break;
        }
    }
}
