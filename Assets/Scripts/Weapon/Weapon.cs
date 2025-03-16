using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Scriptable Object/Weapon", order = 1)]
public class Weapon : Item
{
    [Header("Weapon Data")]
    public float damage;
    public float range;
    public EnumTypes.AmmoType ammoType;
    public int ammoCapacity;

    public EnumTypes.ElementType elementalType;
    public int elementalProb;

    public float fireRate;
    public float reloadTime;

    public Vector2 recoilRate;
    public float recoilSpeed;
    public float returnSpeed;

    public bool isAutomatic;

    [HideInInspector] public bool zoomable;
    [HideInInspector] public Vector2 aimRecoilRate;
    [HideInInspector] public float zoomRate;

    [HideInInspector] public bool spreadable;
    [HideInInspector] public int numOfShell;
    [HideInInspector] public float scatterRate;

    public override void Gain(PlayerController player)
    {
        GameObject go = ObjectPool.Instance.Pop($"Weapon/{ItemName}", player.transform.position, Quaternion.identity);
        go.GetComponent<WeaponPickUp>().BaseInteract(player);
    }
}
