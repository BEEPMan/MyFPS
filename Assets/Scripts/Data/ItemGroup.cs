using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemGroup", menuName = "Scriptable Object/Group/ItemGroup", order = 1)]
public class ItemGroup : ScriptableObject
{
    public List<Weapon> Weapons;
    public List<Scroll> Scrolls;
    public List<Item> PickupItems;
    public List<Item> PeddlerItems;
}
