using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StatData", menuName = "Scriptable Object/StatData", order = 1)]
public class StatData : ScriptableObject
{
    public EnumTypes.CharacterType CharacterType;
    public float Speed;
    [HideInInspector] public EnumTypes.HPType HPType;
    [HideInInspector] public int HP;
    [HideInInspector] public int Shield;
    [HideInInspector] public int Armor;
}
