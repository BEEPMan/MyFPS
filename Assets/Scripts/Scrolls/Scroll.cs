using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scroll", menuName = "Scriptable Object/Scroll", order = 1)]
public class Scroll : ScriptableObject
{
    public Sprite scrollIcon;
    public float givenDamage;
    public float takenDamage;
    public float weaponDamage;
    public float critDamage;
    public float skillDamage;
    public float[] elementalDamage = new float[4];
    public string description;
    public int price;
}
