using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Weapon,
    Scroll,
    AmmoSupply,
    HealthKit,
}

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Object/Item", order = 1)]
public class Item : ScriptableObject
{
    public ItemType type;
    [Header("Item Info")]
    public string itemName;
    public string description;
    public Sprite icon;
    public int price;
    [Header("For Weapon Only")]
    public Vector3 equipedPos;
    public Vector3 equipedRot;
    [Header("For Scroll Only")]
    public Scroll scroll;
    [Header("For AmmoSupply&HealthKit")]
    public int amount;
    [Header("For AmmoSupply Only")]
    public AmmoType ammoType;
}
