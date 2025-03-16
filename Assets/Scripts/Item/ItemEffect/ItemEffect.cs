using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemEffect", menuName = "Scriptable Object/ItemEffect/ItemEffect", order = 1)]
public abstract class ItemEffect : ScriptableObject
{
    public abstract void ExecuteEffect(Item item, PlayerController player);
    public abstract void ScrollDropEffect(Item item, PlayerController player);
}
